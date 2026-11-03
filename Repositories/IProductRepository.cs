using EF_CRUD.Models;

namespace EF_CRUD.Repositories;

public interface IProductRepository : IRepository<Product>
{
    /// <summary>All products with Category and Supplier loaded (read-only).</summary>
    Task<IReadOnlyList<Product>> GetAllWithDetailsAsync();

    /// <summary>One product with Category and Supplier loaded (read-only), or null.</summary>
    Task<Product?> GetByIdWithDetailsAsync(int id);
}
