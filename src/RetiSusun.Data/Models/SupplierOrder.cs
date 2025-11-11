using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class SupplierOrder
{
    [Key]
    public int SupplierOrderId { get; set; }

    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpectedDeliveryDate { get; set; }

    public DateTime? ActualDeliveryDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Preparing, Shipped, Delivered, Cancelled

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? DeliveryAddress { get; set; }

    public int? ProcessedByUserId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastUpdatedDate { get; set; }

    // Navigation properties - which supplier received this order
    [Required]
    public int SupplierId { get; set; }
    [ForeignKey(nameof(SupplierId))]
    public Supplier Supplier { get; set; } = null!;

    // Navigation properties - which business placed this order
    [Required]
    public int BusinessId { get; set; }
    [ForeignKey(nameof(BusinessId))]
    public Business Business { get; set; } = null!;

    public ICollection<SupplierOrderItem> Items { get; set; } = new List<SupplierOrderItem>();
}
