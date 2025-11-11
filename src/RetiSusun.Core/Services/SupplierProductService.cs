using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Core.Services;

public class SupplierProductService : ISupplierProductService
{
    private readonly RetiSusunDbContext _context;

    public SupplierProductService(RetiSusunDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierProduct> CreateProductAsync(SupplierProduct product)
    {
        product.CreatedDate = DateTime.UtcNow;
        _context.SupplierProducts.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<SupplierProduct?> GetProductByIdAsync(int productId)
    {
        return await _context.SupplierProducts
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.SupplierProductId == productId);
    }

    public async Task<IEnumerable<SupplierProduct>> GetProductsBySupplierIdAsync(int supplierId)
    {
        return await _context.SupplierProducts
            .Where(p => p.SupplierId == supplierId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierProduct>> GetActiveProductsBySupplierIdAsync(int supplierId)
    {
        return await _context.SupplierProducts
            .Where(p => p.SupplierId == supplierId && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierProduct>> SearchProductsAsync(string searchTerm)
    {
        return await _context.SupplierProducts
            .Include(p => p.Supplier)
            .Where(p => p.IsActive && p.Supplier.IsOpenForBusiness &&
                       (p.Name.Contains(searchTerm) || 
                        p.Description!.Contains(searchTerm) ||
                        p.Barcode!.Contains(searchTerm) ||
                        p.SKU!.Contains(searchTerm)))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierProduct>> GetProductsByCategoryAsync(string category, int? supplierId = null)
    {
        var query = _context.SupplierProducts
            .Include(p => p.Supplier)
            .Where(p => p.IsActive && p.Category == category);

        if (supplierId.HasValue)
        {
            query = query.Where(p => p.SupplierId == supplierId.Value);
        }
        else
        {
            query = query.Where(p => p.Supplier.IsOpenForBusiness);
        }

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<SupplierProduct> UpdateProductAsync(SupplierProduct product)
    {
        product.LastUpdatedDate = DateTime.UtcNow;
        _context.SupplierProducts.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _context.SupplierProducts.FindAsync(productId);
        if (product == null)
            return false;

        _context.SupplierProducts.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = await _context.SupplierProducts.FindAsync(productId);
        if (product == null)
            return false;

        product.AvailableStock += quantity;
        product.LastUpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SupplierProduct>> GetLowStockProductsAsync(int supplierId, int threshold = 10)
    {
        return await _context.SupplierProducts
            .Where(p => p.SupplierId == supplierId && p.IsActive && p.AvailableStock <= threshold)
            .OrderBy(p => p.AvailableStock)
            .ToListAsync();
    }
}
