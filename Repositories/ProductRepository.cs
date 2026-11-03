using EF_CRUD.Data;
using EF_CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Repositories;

public sealed class ProductRepository(ProductDbContext context)
    : Repository<Product>(context), IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllWithDetailsAsync() =>
        await Context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .OrderBy(p => p.Id)
            .ToListAsync();

    public Task<Product?> GetByIdWithDetailsAsync(int id) =>
        Context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .SingleOrDefaultAsync(p => p.Id == id);
}
