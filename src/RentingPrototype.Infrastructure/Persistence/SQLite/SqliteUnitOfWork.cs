using System.Data;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Infrastructure.Persistence.Sqlite;

namespace RentingPrototype.Infrastructure.Persistence.SQLite;

public class SqliteUnitOfWork : IUnitOfWork
{
    private readonly ISqliteConnectionFactory _factory;

    public IDbConnection? Connection { get; private set; }
    public IDbTransaction? Transaction { get; private set; }

    /// <summary>
    /// Creates a unit-of-work instance backed by SQLite.
    /// </summary>
    /// <param name="factory">SQLite connection factory.</param>
    public SqliteUnitOfWork(ISqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Opens a connection and starts a transaction if one is not already active.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    public Task BeginAsync(CancellationToken token)
    {
        Connection ??= _factory.CreateConnection();
        if (Connection.State != ConnectionState.Open) Connection.Open();

        Transaction ??= Connection.BeginTransaction();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Commits the active transaction and disposes transactional resources.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    public Task CommitAsync(CancellationToken token)
    {
        Transaction?.Commit();
        Cleanup();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Rolls back the active transaction and disposes transactional resources.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    public Task RollbackAsync(CancellationToken token)
    {
        try
        {
            Transaction?.Rollback();
        }
        finally
        {
            Cleanup();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes connection and transaction objects.
    /// </summary>
    private void Cleanup()
    {
        Transaction?.Dispose();
        Transaction = null;

        Connection?.Dispose();
        Connection = null;
    }
}
