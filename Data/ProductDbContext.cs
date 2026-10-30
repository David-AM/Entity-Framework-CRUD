using EF_CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Data;

public sealed class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.Id);
            entity.HasIndex(category => category.Name).IsUnique();
            entity.Property(category => category.Name).IsRequired().HasMaxLength(80);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(supplier => supplier.Id);
            entity.HasIndex(supplier => supplier.Name).IsUnique();
            entity.Property(supplier => supplier.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).IsRequired().HasMaxLength(120);
            entity.Property(product => product.Price).HasColumnType("decimal(18,2)");
            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(product => product.Supplier)
                .WithMany(supplier => supplier.Products)
                .HasForeignKey(product => product.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }
}
