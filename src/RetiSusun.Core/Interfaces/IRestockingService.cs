using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface IRestockingService
{
    Task<RestockingRecord> AddRestockingRecordAsync(RestockingRecord record);
    Task<IEnumerable<RestockingRecord>> GetRestockingHistoryAsync(int productId);
    Task<Dictionary<int, int>> GetRestockingSuggestionsAsync(int businessId);
    Task<bool> ApplyRestockingSuggestionAsync(int productId, int suggestedQuantity, int userId);
}
