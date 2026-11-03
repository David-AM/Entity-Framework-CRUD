using EF_CRUD.Models;

namespace EF_CRUD.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<bool> HasProductsAsync(int id);
}
