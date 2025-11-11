using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Core.Services;

public class SupplierOrderService : ISupplierOrderService
{
    private readonly RetiSusunDbContext _context;

    public SupplierOrderService(RetiSusunDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierOrder> CreateOrderAsync(SupplierOrder order)
    {
        order.OrderNumber = await GenerateOrderNumberAsync();
        order.OrderDate = DateTime.UtcNow;
        order.CreatedDate = DateTime.UtcNow;

        _context.SupplierOrders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<SupplierOrder?> GetOrderByIdAsync(int orderId)
    {
        return await _context.SupplierOrders
            .Include(o => o.Items)
                .ThenInclude(i => i.SupplierProduct)
            .Include(o => o.Supplier)
            .Include(o => o.Business)
            .FirstOrDefaultAsync(o => o.SupplierOrderId == orderId);
    }

    public async Task<IEnumerable<SupplierOrder>> GetOrdersBySupplierIdAsync(int supplierId)
    {
        return await _context.SupplierOrders
            .Include(o => o.Items)
            .Include(o => o.Business)
            .Where(o => o.SupplierId == supplierId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierOrder>> GetOrdersByBusinessIdAsync(int businessId)
    {
        return await _context.SupplierOrders
            .Include(o => o.Items)
            .Include(o => o.Supplier)
            .Where(o => o.BusinessId == businessId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierOrder>> GetOrdersByStatusAsync(int supplierId, string status)
    {
        return await _context.SupplierOrders
            .Include(o => o.Items)
            .Include(o => o.Business)
            .Where(o => o.SupplierId == supplierId && o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<SupplierOrder> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _context.SupplierOrders.FindAsync(orderId);
        if (order == null)
            throw new InvalidOperationException("Order not found");

        order.Status = status;
        order.LastUpdatedDate = DateTime.UtcNow;

        if (status == "Delivered")
        {
            order.ActualDeliveryDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<SupplierOrder> UpdateOrderAsync(SupplierOrder order)
    {
        order.LastUpdatedDate = DateTime.UtcNow;
        _context.SupplierOrders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> CancelOrderAsync(int orderId, string reason)
    {
        var order = await _context.SupplierOrders.FindAsync(orderId);
        if (order == null)
            return false;

        order.Status = "Cancelled";
        order.Notes = string.IsNullOrEmpty(order.Notes) 
            ? $"Cancelled: {reason}" 
            : $"{order.Notes}\nCancelled: {reason}";
        order.LastUpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalSalesBySupplierIdAsync(int supplierId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.SupplierOrders
            .Where(o => o.SupplierId == supplierId && o.Status != "Cancelled");

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        return await query.SumAsync(o => o.TotalAmount);
    }

    public async Task<IEnumerable<SupplierOrder>> GetRecentOrdersAsync(int supplierId, int count = 10)
    {
        return await _context.SupplierOrders
            .Include(o => o.Items)
            .Include(o => o.Business)
            .Where(o => o.SupplierId == supplierId)
            .OrderByDescending(o => o.OrderDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetOrderStatusSummaryAsync(int supplierId)
    {
        return await _context.SupplierOrders
            .Where(o => o.SupplierId == supplierId)
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"SO{date:yyyyMMdd}";
        
        var lastOrder = await _context.SupplierOrders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastOrder != null && lastOrder.OrderNumber.Length > prefix.Length)
        {
            var lastSequence = lastOrder.OrderNumber.Substring(prefix.Length);
            if (int.TryParse(lastSequence, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }
}
