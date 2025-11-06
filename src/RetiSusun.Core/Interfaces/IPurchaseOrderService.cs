using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface IPurchaseOrderService
{
    Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int purchaseOrderId);
    Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(int businessId, string? status = null);
    Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
    Task<bool> ReceivePurchaseOrderAsync(int purchaseOrderId, int userId);
}
