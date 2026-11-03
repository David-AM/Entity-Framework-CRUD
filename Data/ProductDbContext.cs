using EF_CRUD.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Data;

public sealed class ProductDbContext(DbContextOptions<ProductDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Product> Products {get; set;}
    public DbSet<Category> Categories {get; set;}
    public DbSet<Supplier> Suppliers {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Must run first: it configures the Identity tables (users, roles, claims...).
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
    }
}
