using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Core.Services;

public class ProductService : IProductService
{
    private readonly RetiSusunDbContext _context;

    public ProductService(RetiSusunDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(int businessId)
    {
        return await _context.Products
            .Where(p => p.BusinessId == businessId && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await _context.Products.FindAsync(productId);
    }

    public async Task<Product?> GetProductByBarcodeAsync(string barcode, int businessId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Barcode == barcode && p.BusinessId == businessId && p.IsActive);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        product.CreatedDate = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        product.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int businessId)
    {
        return await _context.Products
            .Where(p => p.BusinessId == businessId && p.IsActive && p.StockQuantity <= p.ReorderLevel)
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int businessId)
    {
        searchTerm = searchTerm.ToLower();
        return await _context.Products
            .Where(p => p.BusinessId == businessId && p.IsActive &&
                       (p.Name.ToLower().Contains(searchTerm) ||
                        p.Barcode!.Contains(searchTerm) ||
                        p.SKU!.Contains(searchTerm)))
            .ToListAsync();
    }
}
