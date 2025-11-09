using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;
using System.Text;

namespace RetiSusun.Core.Services;

public class SalesService : ISalesService
{
    private readonly RetiSusunDbContext _context;
    private readonly IProductService _productService;

    public SalesService(RetiSusunDbContext context, IProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<SalesTransaction> CreateSalesTransactionAsync(SalesTransaction transaction)
    {
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Generate transaction number
            transaction.TransactionNumber = await GenerateTransactionNumberAsync(transaction.BusinessId);
            transaction.TransactionDate = DateTime.UtcNow;

            // Add transaction
            _context.SalesTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Update product stock
            foreach (var item in transaction.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity < 0)
                        throw new InvalidOperationException($"Insufficient stock for product: {product.Name}");
                }
            }

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return transaction;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SalesTransaction?> GetSalesTransactionByIdAsync(int transactionId)
    {
        return await _context.SalesTransactions
            .Include(st => st.Items)
                .ThenInclude(sti => sti.Product)
            .Include(st => st.User)
            .FirstOrDefaultAsync(st => st.TransactionId == transactionId);
    }

    public async Task<IEnumerable<SalesTransaction>> GetSalesTransactionsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.SalesTransactions
            .Include(st => st.Items)
            .Include(st => st.User)
            .Where(st => st.BusinessId == businessId && !st.IsVoided);

        if (startDate.HasValue)
            query = query.Where(st => st.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(st => st.TransactionDate <= endDate.Value);

        return await query
            .OrderByDescending(st => st.TransactionDate)
            .ToListAsync();
    }

    public async Task<bool> VoidTransactionAsync(int transactionId, string reason, int userId)
    {
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var transaction = await _context.SalesTransactions
                .Include(st => st.Items)
                .FirstOrDefaultAsync(st => st.TransactionId == transactionId);

            if (transaction == null || transaction.IsVoided)
                return false;

            // Restore stock
            foreach (var item in transaction.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            transaction.IsVoided = true;
            transaction.VoidedDate = DateTime.UtcNow;
            transaction.VoidReason = reason;

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return true;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<decimal> GetTotalSalesAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.SalesTransactions
            .Where(st => st.BusinessId == businessId && !st.IsVoided);

        if (startDate.HasValue)
            query = query.Where(st => st.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(st => st.TransactionDate <= endDate.Value);

        return await query.SumAsync(st => st.TotalAmount);
    }

    public async Task<string> GenerateReceiptAsync(int transactionId)
    {
        var transaction = await GetSalesTransactionByIdAsync(transactionId);
        if (transaction == null)
            return string.Empty;

        var receipt = new StringBuilder();
        receipt.AppendLine("========================================");
        receipt.AppendLine("           SALES RECEIPT");
        receipt.AppendLine("========================================");
        receipt.AppendLine($"Transaction #: {transaction.TransactionNumber}");
        receipt.AppendLine($"Date: {transaction.TransactionDate:yyyy-MM-dd HH:mm:ss}");
        receipt.AppendLine($"Cashier: {transaction.User.FullName}");
        receipt.AppendLine("========================================");
        receipt.AppendLine("ITEMS:");
        receipt.AppendLine("----------------------------------------");

        foreach (var item in transaction.Items)
        {
            receipt.AppendLine($"{item.Product.Name}");
            receipt.AppendLine($"  {item.Quantity} x {item.UnitPrice:C} = {item.TotalPrice:C}");
        }

        receipt.AppendLine("========================================");
        receipt.AppendLine($"Subtotal:        {transaction.SubTotal:C}");
        if (transaction.TaxAmount > 0)
            receipt.AppendLine($"Tax ({transaction.TaxRate}%):    {transaction.TaxAmount:C}");
        if (transaction.DiscountAmount > 0)
            receipt.AppendLine($"Discount:       -{transaction.DiscountAmount:C}");
        receipt.AppendLine($"TOTAL:           {transaction.TotalAmount:C}");
        receipt.AppendLine($"Payment Method:  {transaction.PaymentMethod}");
        receipt.AppendLine($"Amount Paid:     {transaction.AmountPaid:C}");
        receipt.AppendLine($"Change:          {transaction.ChangeAmount:C}");
        receipt.AppendLine("========================================");
        receipt.AppendLine("       Thank you for your business!");
        receipt.AppendLine("========================================");

        return receipt.ToString();
    }

    private async Task<string> GenerateTransactionNumberAsync(int businessId)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.SalesTransactions
            .Where(st => st.BusinessId == businessId && st.TransactionDate >= today)
            .CountAsync();

        return $"TXN-{today:yyyyMMdd}-{count + 1:D4}";
    }
}
