using EF_CRUD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Data;

public sealed class ProductDbContext(DbContextOptions<ProductDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Must run first: it configures the Identity tables (users, roles, claims...).
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(builder =>
        {
            builder.ToTable("Categories");
            builder.HasKey(category => category.Id);
            builder.Property(category => category.Name)
                .IsRequired()
                .HasMaxLength(80);
            builder.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<Supplier>(builder =>
        {
            builder.ToTable("Suppliers");
            builder.HasKey(supplier => supplier.Id);
            builder.Property(supplier => supplier.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(supplier => supplier.Name).IsUnique();
        });

        modelBuilder.Entity<Product>(builder =>
        {
            builder.ToTable("Products");
            builder.HasKey(product => product.Id);
            builder.Property(product => product.Name)
                .IsRequired()
                .HasMaxLength(120);
            builder.Property(product => product.Price)
                .HasColumnType("decimal(18,2)");
            builder.Property(product => product.Description)
                .HasMaxLength(500);

            builder.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(product => product.Supplier)
                .WithMany(supplier => supplier.Products)
                .HasForeignKey(product => product.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
