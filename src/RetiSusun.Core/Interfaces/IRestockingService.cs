using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface IRestockingService
{
    Task<RestockingRecord> AddRestockingRecordAsync(RestockingRecord record);
    Task<IEnumerable<RestockingRecord>> GetRestockingHistoryAsync(int productId);
    Task<Dictionary<int, int>> GetRestockingSuggestionsAsync(int businessId);
    Task<bool> ApplyRestockingSuggestionAsync(int productId, int suggestedQuantity, int userId);
    Task<List<SupplierRecommendation>> GetSupplierRecommendationsAsync(int businessId);
}

public class SupplierRecommendation
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    public int ProductsNeeded { get; set; }
    public int TotalProductsNeeded { get; set; }
    public decimal CoveragePercentage { get; set; }
    public decimal EstimatedCost { get; set; }
    public List<ProductMatch> MatchingProducts { get; set; } = new();
}

public class ProductMatch
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int SupplierProductId { get; set; }
    public int SuggestedQuantity { get; set; }
    public decimal UnitPrice { get; set; }
}
