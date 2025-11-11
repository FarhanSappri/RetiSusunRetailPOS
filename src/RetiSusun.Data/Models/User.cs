using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetiSusun.Data.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "Cashier"; // Admin, Manager, Cashier

    [Required]
    [MaxLength(50)]
    public string AccountType { get; set; } = "Business"; // Business, Supplier

    public bool DarkModeEnabled { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginDate { get; set; }

    // Navigation property
    public int? BusinessId { get; set; }
    [ForeignKey(nameof(BusinessId))]
    public Business? Business { get; set; }

    public int? SupplierId { get; set; }
    [ForeignKey(nameof(SupplierId))]
    public Supplier? Supplier { get; set; }

    public ICollection<SalesTransaction> SalesTransactions { get; set; } = new List<SalesTransaction>();
}
