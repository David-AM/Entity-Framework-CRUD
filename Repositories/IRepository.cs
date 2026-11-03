namespace EF_CRUD.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<bool> ExistsAsync(int id);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void Add(T entity);
    void Remove(T entity);
}
