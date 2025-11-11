using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Barcode { get; set; }

    [MaxLength(50)]
    public string? SKU { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; }

    [Required]
    public int StockQuantity { get; set; }

    public int MinimumStockLevel { get; set; } = 10;

    public int ReorderLevel { get; set; } = 20;

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(100)]
    public string? Brand { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; } = "pcs";

    [MaxLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastRestockedDate { get; set; }

    // Navigation property
    [Required]
    public int BusinessId { get; set; }
    [ForeignKey(nameof(BusinessId))]
    public Business Business { get; set; } = null!;

    public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; } = new List<SalesTransactionItem>();
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<RestockingRecord> RestockingRecords { get; set; } = new List<RestockingRecord>();
}
