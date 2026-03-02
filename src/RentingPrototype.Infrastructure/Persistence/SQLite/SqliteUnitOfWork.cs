using System.Data;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Infrastructure.Persistence.Sqlite;

namespace RentingPrototype.Infrastructure.Persistence.SQLite;

public class SqliteUnitOfWork : IUnitOfWork
{
    private readonly ISqliteConnectionFactory _factory;

    public IDbConnection? Connection { get; private set; }
    public IDbTransaction? Transaction { get; private set; }

    public SqliteUnitOfWork(ISqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public Task BeginAsync(CancellationToken token)
    {
        Connection ??= _factory.CreateConnection();
        if (Connection.State != ConnectionState.Open) Connection.Open();

        Transaction ??= Connection.BeginTransaction();
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken token)
    {
        Transaction?.Commit();
        Cleanup();
        return Task.CompletedTask;
    }

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

    private void Cleanup()
    {
        Transaction?.Dispose();
        Transaction = null;

        Connection?.Dispose();
        Connection = null;
    }
}
