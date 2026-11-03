using EF_CRUD.Data;
using EF_CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Repositories;

public sealed class CategoryRepository(ProductDbContext context)
    : Repository<Category>(context), ICategoryRepository
{
    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null) =>
        Context.Categories.AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId));

    public Task<bool> HasProductsAsync(int id) =>
        Context.Products.AnyAsync(p => p.CategoryId == id);
}
