using InventoryAPIRest.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPIRest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Id).ValueGeneratedOnAdd();
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
                e.Property(c => c.Description).HasMaxLength(300);
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.Property(p => p.Name).IsRequired().HasMaxLength(150);
                e.Property(p => p.Code).IsRequired().HasMaxLength(50);
                e.HasIndex(p => p.Code).IsUnique();
                e.Property(p => p.Description).HasMaxLength(500);
                e.Property(p => p.Price).HasPrecision(18, 2);

                e.HasOne(p => p.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(p => p.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryMovement>(e =>
            {
                e.HasKey(m => m.Id);
                e.Property(m => m.Id).ValueGeneratedOnAdd();
                e.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);
                e.Property(m => m.Reason).HasMaxLength(300);
                e.Property(m => m.UnitPrice).HasPrecision(18, 2);
                e.HasIndex(m => new { m.ProductId, m.CreatedAt });

                e.HasOne(m => m.Product)
                 .WithMany(p => p.Movements)
                 .HasForeignKey(m => m.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
