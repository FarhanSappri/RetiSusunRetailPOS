using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class RestockingRecord
{
    [Key]
    public int RestockingRecordId { get; set; }

    public DateTime RestockDate { get; set; } = DateTime.UtcNow;

    [Required]
    public int QuantityAdded { get; set; }

    public int StockBeforeRestock { get; set; }

    public int StockAfterRestock { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCost { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(50)]
    public string Source { get; set; } = "Manual"; // Manual, PurchaseOrder

    public int? PurchaseOrderId { get; set; }

    public int RestockedByUserId { get; set; }

    // Navigation properties
    [Required]
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}
