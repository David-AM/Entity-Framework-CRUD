using EF_CRUD.Data;
using EF_CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Repositories;

public sealed class SupplierRepository(ProductDbContext context)
    : Repository<Supplier>(context), ISupplierRepository
{
    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null) =>
        Context.Suppliers.AnyAsync(s => s.Name == name && (excludeId == null || s.Id != excludeId));

    public Task<bool> HasProductsAsync(int id) =>
        Context.Products.AnyAsync(p => p.SupplierId == id);
}
