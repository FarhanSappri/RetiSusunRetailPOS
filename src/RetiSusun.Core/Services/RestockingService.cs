using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Core.Services;

public class RestockingService : IRestockingService
{
    private readonly RetiSusunDbContext _context;

    public RestockingService(RetiSusunDbContext context)
    {
        _context = context;
    }

    public async Task<RestockingRecord> AddRestockingRecordAsync(RestockingRecord record)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = await _context.Products.FindAsync(record.ProductId);
            if (product == null)
                throw new InvalidOperationException("Product not found");

            record.StockBeforeRestock = product.StockQuantity;
            record.StockAfterRestock = product.StockQuantity + record.QuantityAdded;
            record.RestockDate = DateTime.UtcNow;

            product.StockQuantity = record.StockAfterRestock;
            product.LastRestockedDate = DateTime.UtcNow;

            _context.RestockingRecords.Add(record);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return record;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<RestockingRecord>> GetRestockingHistoryAsync(int productId)
    {
        return await _context.RestockingRecords
            .Where(rr => rr.ProductId == productId)
            .OrderByDescending(rr => rr.RestockDate)
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetRestockingSuggestionsAsync(int businessId)
    {
        var suggestions = new Dictionary<int, int>();

        // Get all products that are below reorder level
        var lowStockProducts = await _context.Products
            .Where(p => p.BusinessId == businessId && p.IsActive && p.StockQuantity <= p.ReorderLevel)
            .ToListAsync();

        foreach (var product in lowStockProducts)
        {
            // Calculate average daily sales for the last 30 days
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            
            var salesData = await _context.SalesTransactionItems
                .Include(sti => sti.SalesTransaction)
                .Where(sti => sti.ProductId == product.ProductId &&
                             sti.SalesTransaction.TransactionDate >= thirtyDaysAgo &&
                             !sti.SalesTransaction.IsVoided)
                .ToListAsync();

            if (salesData.Any())
            {
                var totalSold = salesData.Sum(sd => sd.Quantity);
                var daysWithSales = salesData
                    .Select(sd => sd.SalesTransaction.TransactionDate.Date)
                    .Distinct()
                    .Count();

                var averageDailySales = daysWithSales > 0 ? (double)totalSold / daysWithSales : 0;

                // Suggest enough stock for 30 days plus safety buffer
                var suggestedQuantity = (int)Math.Ceiling(averageDailySales * 30 * 1.2); // 20% buffer
                
                // Ensure minimum restock quantity
                suggestedQuantity = Math.Max(suggestedQuantity, product.ReorderLevel * 2);
                
                // Calculate how much to order to reach suggested stock level
                var orderQuantity = Math.Max(0, suggestedQuantity - product.StockQuantity);

                if (orderQuantity > 0)
                {
                    suggestions[product.ProductId] = orderQuantity;
                }
            }
            else
            {
                // No sales history, suggest standard restock amount
                suggestions[product.ProductId] = product.ReorderLevel * 2;
            }
        }

        return suggestions;
    }

    public async Task<bool> ApplyRestockingSuggestionAsync(int productId, int suggestedQuantity, int userId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        var record = new RestockingRecord
        {
            ProductId = productId,
            QuantityAdded = suggestedQuantity,
            UnitCost = product.CostPrice,
            TotalCost = product.CostPrice * suggestedQuantity,
            RestockedByUserId = userId,
            Source = "Suggestion",
            Notes = "Applied from restocking suggestion engine"
        };

        await AddRestockingRecordAsync(record);
        return true;
    }
    
    public async Task<List<SupplierRecommendation>> GetSupplierRecommendationsAsync(int businessId)
    {
        // Get restocking suggestions first
        var suggestions = await GetRestockingSuggestionsAsync(businessId);
        if (!suggestions.Any())
            return new List<SupplierRecommendation>();
        
        var recommendations = new List<SupplierRecommendation>();
        
        // Get all active suppliers
        var suppliers = await _context.Suppliers
            .Where(s => s.IsActive && s.IsOpenForBusiness)
            .ToListAsync();
        
        // Get all products that need restocking
        var productIds = suggestions.Keys.ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId, p => p);
        
        // For each supplier, find matching products
        foreach (var supplier in suppliers)
        {
            var supplierProducts = await _context.SupplierProducts
                .Where(sp => sp.SupplierId == supplier.SupplierId && sp.IsActive)
                .ToListAsync();
            
            var matchingProducts = new List<ProductMatch>();
            decimal estimatedCost = 0;
            
            foreach (var suggestion in suggestions)
            {
                if (!products.ContainsKey(suggestion.Key))
                    continue;
                
                var product = products[suggestion.Key];
                
                // Try to find matching supplier product by name or SKU
                var supplierProduct = supplierProducts.FirstOrDefault(sp =>
                    sp.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(product.SKU) && !string.IsNullOrEmpty(sp.SKU) &&
                     sp.SKU.Equals(product.SKU, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(product.Barcode) && !string.IsNullOrEmpty(sp.Barcode) &&
                     sp.Barcode.Equals(product.Barcode, StringComparison.OrdinalIgnoreCase)));
                
                if (supplierProduct != null)
                {
                    matchingProducts.Add(new ProductMatch
                    {
                        ProductId = product.ProductId,
                        ProductName = product.Name,
                        SupplierProductId = supplierProduct.SupplierProductId,
                        SuggestedQuantity = suggestion.Value,
                        UnitPrice = supplierProduct.WholesalePrice
                    });
                    
                    estimatedCost += supplierProduct.WholesalePrice * suggestion.Value;
                }
            }
            
            if (matchingProducts.Any())
            {
                recommendations.Add(new SupplierRecommendation
                {
                    SupplierId = supplier.SupplierId,
                    SupplierName = supplier.CompanyName,
                    LogoPath = supplier.LogoPath,
                    ProductsNeeded = matchingProducts.Count,
                    TotalProductsNeeded = suggestions.Count,
                    CoveragePercentage = (decimal)matchingProducts.Count / suggestions.Count * 100,
                    EstimatedCost = estimatedCost,
                    MatchingProducts = matchingProducts
                });
            }
        }
        
        // Sort by coverage percentage (highest first) and then by estimated cost (lowest first)
        return recommendations
            .OrderByDescending(r => r.CoveragePercentage)
            .ThenBy(r => r.EstimatedCost)
            .ToList();
    }
}
