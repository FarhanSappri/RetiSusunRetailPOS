using System.ComponentModel.DataAnnotations;

namespace RetiSusun.Data.Models;

public class Business
{
    [Key]
    public int BusinessId { get; set; }

    [Required]
    [MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    [MaxLength(100)]
    public string OwnerName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? OwnerEmail { get; set; }

    [MaxLength(20)]
    public string? OwnerPhone { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<SalesTransaction> SalesTransactions { get; set; } = new List<SalesTransaction>();
}
