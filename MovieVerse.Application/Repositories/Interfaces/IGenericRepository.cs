using System.Linq.Expressions;
using MovieVerse.Models.Common;

namespace MovieVerse.Repositories.Interfaces;

public interface IGenericRepository<T>
    where T : BaseEntity
{
    Task<List<T>> GetAllAsync();

    Task<T?> GetByIdAsync(Guid id);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null);

    Task<List<T>> FindAllAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking = false,
        params string[] includes);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking = false,
        params string[] includes);


    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task SaveChangesAsync();
}