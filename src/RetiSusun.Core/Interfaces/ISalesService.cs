using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface ISalesService
{
    Task<SalesTransaction> CreateSalesTransactionAsync(SalesTransaction transaction);
    Task<SalesTransaction?> GetSalesTransactionByIdAsync(int transactionId);
    Task<IEnumerable<SalesTransaction>> GetSalesTransactionsAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> VoidTransactionAsync(int transactionId, string reason, int userId);
    Task<decimal> GetTotalSalesAsync(int businessId, DateTime? startDate = null, DateTime? endDate = null);
    Task<string> GenerateReceiptAsync(int transactionId);
}
