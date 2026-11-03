using EF_CRUD.Data;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Repositories;

public class Repository<T>(ProductDbContext context) : IRepository<T> where T : class
{
    protected ProductDbContext Context { get; } = context;
    protected DbSet<T> Set => Context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id) => await Set.FindAsync(id);

    // Read-only list, so no change tracking.
    public virtual async Task<IReadOnlyList<T>> GetAllAsync() => await Set.AsNoTracking().ToListAsync();

    public virtual Task<bool> ExistsAsync(int id) =>
        Set.AnyAsync(e => EF.Property<int>(e, "Id") == id);
    public virtual void Add(T entity) => Set.Add(entity);
    public virtual void Remove(T entity) => Set.Remove(entity);
}
