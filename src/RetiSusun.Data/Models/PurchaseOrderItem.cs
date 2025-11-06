using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class PurchaseOrderItem
{
    [Key]
    public int PurchaseOrderItemId { get; set; }

    [Required]
    public int QuantityOrdered { get; set; }

    public int QuantityReceived { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCost { get; set; }

    // Navigation properties
    [Required]
    public int PurchaseOrderId { get; set; }
    [ForeignKey(nameof(PurchaseOrderId))]
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}
