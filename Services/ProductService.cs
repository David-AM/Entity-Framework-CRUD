using EF_CRUD.Data;
using EF_CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Services;

public sealed class ProductService(ProductDbContext context)
{
    public Task<List<Product>> GetAllAsync() => context.Products.Include(p => p.Category)
                                                                .Include(p => p.Supplier)
                                                                .OrderBy(p => p.Id).ToListAsync();
    public Task<Product?> GetByIdAsync(int id) => context.Products.Include(p => p.Category)
                                                                    .Include(p => p.Supplier)
                                                                    .SingleOrDefaultAsync(p => p.Id == id);

    public async Task<Product> CreateAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        var existing = await context.Products.FindAsync(id);
        if (existing is null) return null;
        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Description = product.Description;
        existing.CategoryId = product.CategoryId;
        existing.SupplierId = product.SupplierId;
        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product is null) return false;
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return true;
    }
}
