using MovieVerse.Models.Common;

namespace MovieVerse.Repositories.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);

    IQueryable<T> Query();

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task SaveChangesAsync();
}