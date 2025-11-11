using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface ISupplierOrderService
{
    Task<SupplierOrder> CreateOrderAsync(SupplierOrder order);
    Task<SupplierOrder?> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<SupplierOrder>> GetOrdersBySupplierIdAsync(int supplierId);
    Task<IEnumerable<SupplierOrder>> GetOrdersByBusinessIdAsync(int businessId);
    Task<IEnumerable<SupplierOrder>> GetOrdersByStatusAsync(int supplierId, string status);
    Task<SupplierOrder> UpdateOrderStatusAsync(int orderId, string status);
    Task<SupplierOrder> UpdateOrderAsync(SupplierOrder order);
    Task<bool> CancelOrderAsync(int orderId, string reason);
    Task<decimal> GetTotalSalesBySupplierIdAsync(int supplierId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<SupplierOrder>> GetRecentOrdersAsync(int supplierId, int count = 10);
    Task<Dictionary<string, int>> GetOrderStatusSummaryAsync(int supplierId);
}
