using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class SalesTransactionItem
{
    [Key]
    public int TransactionItemId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    // Navigation properties
    [Required]
    public int TransactionId { get; set; }
    [ForeignKey(nameof(TransactionId))]
    public SalesTransaction SalesTransaction { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
}
