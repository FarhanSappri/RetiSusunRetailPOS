using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class SupplierOrderItem
{
    [Key]
    public int SupplierOrderItemId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    // Navigation properties
    [Required]
    public int SupplierOrderId { get; set; }
    [ForeignKey(nameof(SupplierOrderId))]
    public SupplierOrder SupplierOrder { get; set; } = null!;

    [Required]
    public int SupplierProductId { get; set; }
    [ForeignKey(nameof(SupplierProductId))]
    public SupplierProduct SupplierProduct { get; set; } = null!;
}
