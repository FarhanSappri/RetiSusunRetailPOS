using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class SupplierProduct
{
    [Key]
    public int SupplierProductId { get; set; }

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
    public decimal WholesalePrice { get; set; }

    [Required]
    public int MinimumOrderQuantity { get; set; } = 1;

    [Required]
    public int AvailableStock { get; set; }

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

    public DateTime? LastUpdatedDate { get; set; }

    // Navigation property
    [Required]
    public int SupplierId { get; set; }
    [ForeignKey(nameof(SupplierId))]
    public Supplier Supplier { get; set; } = null!;

    public ICollection<SupplierOrderItem> SupplierOrderItems { get; set; } = new List<SupplierOrderItem>();
}
