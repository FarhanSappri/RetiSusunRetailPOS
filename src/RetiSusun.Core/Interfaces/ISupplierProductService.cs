using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface ISupplierProductService
{
    Task<SupplierProduct> CreateProductAsync(SupplierProduct product);
    Task<SupplierProduct?> GetProductByIdAsync(int productId);
    Task<IEnumerable<SupplierProduct>> GetProductsBySupplierIdAsync(int supplierId);
    Task<IEnumerable<SupplierProduct>> GetActiveProductsBySupplierIdAsync(int supplierId);
    Task<IEnumerable<SupplierProduct>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<SupplierProduct>> GetProductsByCategoryAsync(string category, int? supplierId = null);
    Task<SupplierProduct> UpdateProductAsync(SupplierProduct product);
    Task<bool> DeleteProductAsync(int productId);
    Task<bool> UpdateStockAsync(int productId, int quantity);
    Task<IEnumerable<SupplierProduct>> GetLowStockProductsAsync(int supplierId, int threshold = 10);
}
