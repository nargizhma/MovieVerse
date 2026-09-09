namespace MovieVerse.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();

    Task<IAppTransaction>
        BeginTransactionAsync();
}

public interface IAppTransaction
    : IAsyncDisposable
{
    Task CommitAsync();

    Task RollbackAsync();
}