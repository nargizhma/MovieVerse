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

    public IQueryable<T> Query()
    {
        return context.Set<T>();
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