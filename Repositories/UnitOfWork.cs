using EF_CRUD.Data;

namespace EF_CRUD.Repositories;

public sealed class UnitOfWork(
    ProductDbContext context,
    IProductRepository products,
    ICategoryRepository categories,
    ISupplierRepository suppliers) : IUnitOfWork
{
    public IProductRepository Products => products;
    public ICategoryRepository Categories => categories;
    public ISupplierRepository Suppliers => suppliers;

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
