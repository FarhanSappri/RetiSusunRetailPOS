using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface ISupplierService
{
    Task<Supplier> CreateSupplierAsync(Supplier supplier);
    Task<Supplier?> GetSupplierByIdAsync(int supplierId);
    Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
    Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
    Task<IEnumerable<Supplier>> GetOpenForBusinessSuppliersAsync();
    Task<Supplier> UpdateSupplierAsync(Supplier supplier);
    Task<bool> DeleteSupplierAsync(int supplierId);
    Task<bool> ToggleSupplierStatusAsync(int supplierId, bool isActive);
    Task<bool> ToggleOpenForBusinessAsync(int supplierId, bool isOpenForBusiness);
}
