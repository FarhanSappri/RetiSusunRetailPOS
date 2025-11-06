using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync(int businessId);
    Task<Product?> GetProductByIdAsync(int productId);
    Task<Product?> GetProductByBarcodeAsync(string barcode, int businessId);
    Task<Product> AddProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int productId);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int businessId);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int businessId);
}
