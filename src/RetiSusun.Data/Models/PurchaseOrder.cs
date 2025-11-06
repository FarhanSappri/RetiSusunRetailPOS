using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class PurchaseOrder
{
    [Key]
    public int PurchaseOrderId { get; set; }

    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpectedDeliveryDate { get; set; }

    public DateTime? ActualDeliveryDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Ordered, Received, Cancelled

    [MaxLength(200)]
    public string? SupplierName { get; set; }

    [MaxLength(100)]
    public string? SupplierEmail { get; set; }

    [MaxLength(20)]
    public string? SupplierPhone { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int CreatedByUserId { get; set; }

    public int? ReceivedByUserId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [Required]
    public int BusinessId { get; set; }
    [ForeignKey(nameof(BusinessId))]
    public Business Business { get; set; } = null!;

    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
