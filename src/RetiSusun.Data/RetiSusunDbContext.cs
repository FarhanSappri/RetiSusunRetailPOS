using Microsoft.EntityFrameworkCore;
using RetiSusun.Data.Models;

namespace RetiSusun.Data;

public class RetiSusunDbContext : DbContext
{
    public RetiSusunDbContext(DbContextOptions<RetiSusunDbContext> options)
        : base(options)
    {
    }

    public DbSet<Business> Businesses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<SalesTransaction> SalesTransactions { get; set; }
    public DbSet<SalesTransactionItem> SalesTransactionItems { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<RestockingRecord> RestockingRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Barcode);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU);

        modelBuilder.Entity<SalesTransaction>()
            .HasIndex(st => st.TransactionNumber)
            .IsUnique();

        modelBuilder.Entity<PurchaseOrder>()
            .HasIndex(po => po.OrderNumber)
            .IsUnique();

        // Configure decimal precision
        modelBuilder.Entity<Product>()
            .Property(p => p.CostPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.SellingPrice)
            .HasPrecision(18, 2);

        // Configure delete behavior
        modelBuilder.Entity<SalesTransaction>()
            .HasMany(st => st.Items)
            .WithOne(sti => sti.SalesTransaction)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseOrder>()
            .HasMany(po => po.Items)
            .WithOne(poi => poi.PurchaseOrder)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
