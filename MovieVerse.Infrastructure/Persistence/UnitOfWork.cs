using Microsoft.EntityFrameworkCore.Storage;
using MovieVerse.Abstractions.Persistence;
using MovieVerse.Data;

namespace MovieVerse.Infrastructure.Persistence;

public class UnitOfWork(
    AppDbContext context)
    : IUnitOfWork
{
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public async Task<IAppTransaction>
        BeginTransactionAsync()
    {
        var transaction =
            await context.Database
                .BeginTransactionAsync();

        return new AppTransaction(
            transaction);
    }

    private sealed class AppTransaction(
        IDbContextTransaction transaction)
        : IAppTransaction
    {
        public async Task CommitAsync()
        {
            await transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await transaction.RollbackAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await transaction.DisposeAsync();
        }
    }
}