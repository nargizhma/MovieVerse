using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MovieVerse.Data;
using MovieVerse.Models.Common;
using MovieVerse.Repositories.Interfaces;

namespace MovieVerse.Repositories;

public class GenericRepository<T>(
    AppDbContext context)
    : IGenericRepository<T>
    where T : BaseEntity
{
    public async Task<List<T>> GetAllAsync()
    {
        return await context.Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>()
            .AnyAsync(predicate);
    }
    public async Task<int> CountAsync(
    Expression<Func<T, bool>>? predicate = null)
    {
        var query =
            context.Set<T>().AsQueryable();

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.CountAsync();
    }
    public async Task<List<T>> FindAllAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking = false,
        params string[] includes)
    {
        IQueryable<T> query =
            context.Set<T>();

        foreach (var include in includes)
            query = query.Include(include);

        query = query.Where(predicate);

        if (!tracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking = false,
        params string[] includes)
    {
        IQueryable<T> query =
            context.Set<T>();

        foreach (var include in includes)
            query = query.Include(include);

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(
            predicate);
    }


    public async Task AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        context.Set<T>().Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}