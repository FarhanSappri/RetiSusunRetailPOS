using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class SalesTransaction
{
    [Key]
    public int TransactionId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TransactionNumber { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; } = 0.00m; // Tax rate as percentage (e.g., 6.00 for 6%)

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, E-Wallet

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ChangeAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsVoided { get; set; } = false;

    public DateTime? VoidedDate { get; set; }

    [MaxLength(500)]
    public string? VoidReason { get; set; }

    // Navigation properties
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    public int BusinessId { get; set; }
    [ForeignKey(nameof(BusinessId))]
    public Business Business { get; set; } = null!;

    public ICollection<SalesTransactionItem> Items { get; set; } = new List<SalesTransactionItem>();
}
