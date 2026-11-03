namespace EF_CRUD.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<bool> ExistsAsync(int id);
    void Add(T entity);
    void Remove(T entity);
}
