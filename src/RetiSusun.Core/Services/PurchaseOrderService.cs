using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Core.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly RetiSusunDbContext _context;

    public PurchaseOrderService(RetiSusunDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        purchaseOrder.OrderNumber = await GenerateOrderNumberAsync(purchaseOrder.BusinessId);
        purchaseOrder.OrderDate = DateTime.UtcNow;
        purchaseOrder.Status = "Pending";
        purchaseOrder.CreatedDate = DateTime.UtcNow;

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync();

        return purchaseOrder;
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int purchaseOrderId)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
                .ThenInclude(poi => poi.Product)
            .FirstOrDefaultAsync(po => po.PurchaseOrderId == purchaseOrderId);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(int businessId, string? status = null)
    {
        var query = _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.BusinessId == businessId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(po => po.Status == status);

        return await query
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync();
    }

    public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        _context.PurchaseOrders.Update(purchaseOrder);
        await _context.SaveChangesAsync();
        return purchaseOrder;
    }

    public async Task<bool> ReceivePurchaseOrderAsync(int purchaseOrderId, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Items)
                    .ThenInclude(poi => poi.Product)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == purchaseOrderId);

            if (purchaseOrder == null || purchaseOrder.Status != "Ordered")
                return false;

            // Update stock for each item
            foreach (var item in purchaseOrder.Items)
            {
                item.Product.StockQuantity += item.QuantityOrdered;
                item.Product.LastRestockedDate = DateTime.UtcNow;
                item.QuantityReceived = item.QuantityOrdered;

                // Create restocking record
                var restockingRecord = new RestockingRecord
                {
                    ProductId = item.ProductId,
                    QuantityAdded = item.QuantityOrdered,
                    StockBeforeRestock = item.Product.StockQuantity - item.QuantityOrdered,
                    StockAfterRestock = item.Product.StockQuantity,
                    UnitCost = item.UnitCost,
                    TotalCost = item.TotalCost,
                    RestockedByUserId = userId,
                    Source = "PurchaseOrder",
                    PurchaseOrderId = purchaseOrderId,
                    Notes = $"Received from PO: {purchaseOrder.OrderNumber}"
                };

                _context.RestockingRecords.Add(restockingRecord);
            }

            purchaseOrder.Status = "Received";
            purchaseOrder.ActualDeliveryDate = DateTime.UtcNow;
            purchaseOrder.ReceivedByUserId = userId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<string> GenerateOrderNumberAsync(int businessId)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.PurchaseOrders
            .Where(po => po.BusinessId == businessId && po.OrderDate >= today)
            .CountAsync();

        return $"PO-{today:yyyyMMdd}-{count + 1:D4}";
    }
}
