namespace EF_CRUD.Repositories;

/// <summary>
/// Groups the repositories so they share one DbContext, and saves all pending changes in a single transaction.
/// </summary>
public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ISupplierRepository Suppliers { get; }

    Task<int> SaveChangesAsync();
}
