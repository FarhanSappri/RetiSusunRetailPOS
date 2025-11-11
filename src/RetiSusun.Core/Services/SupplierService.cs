using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Core.Services;

public class SupplierService : ISupplierService
{
    private readonly RetiSusunDbContext _context;

    public SupplierService(RetiSusunDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
    {
        supplier.CreatedDate = DateTime.UtcNow;
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int supplierId)
    {
        return await _context.Suppliers
            .Include(s => s.SupplierProducts)
            .FirstOrDefaultAsync(s => s.SupplierId == supplierId);
    }

    public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
    {
        return await _context.Suppliers
            .OrderBy(s => s.CompanyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
    {
        return await _context.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.CompanyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Supplier>> GetOpenForBusinessSuppliersAsync()
    {
        return await _context.Suppliers
            .Where(s => s.IsActive && s.IsOpenForBusiness)
            .OrderBy(s => s.CompanyName)
            .ToListAsync();
    }

    public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
    {
        supplier.LastUpdatedDate = DateTime.UtcNow;
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task<bool> DeleteSupplierAsync(int supplierId)
    {
        var supplier = await _context.Suppliers.FindAsync(supplierId);
        if (supplier == null)
            return false;

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleSupplierStatusAsync(int supplierId, bool isActive)
    {
        var supplier = await _context.Suppliers.FindAsync(supplierId);
        if (supplier == null)
            return false;

        supplier.IsActive = isActive;
        supplier.LastUpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleOpenForBusinessAsync(int supplierId, bool isOpenForBusiness)
    {
        var supplier = await _context.Suppliers.FindAsync(supplierId);
        if (supplier == null)
            return false;

        supplier.IsOpenForBusiness = isOpenForBusiness;
        supplier.LastUpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
