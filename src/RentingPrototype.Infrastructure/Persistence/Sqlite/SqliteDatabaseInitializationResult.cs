using Microsoft.Data.Sqlite;

namespace RentingPrototype.Infrastructure.Persistence.Sqlite;

public sealed class SqliteDatabaseInitializationResult : IDisposable
{
    public string ConnectionString { get; }
    public SqliteConnection? InMemoryKeepAliveConnection { get; }

    public SqliteDatabaseInitializationResult(
        string connectionString,
        SqliteConnection? inMemoryKeepAliveConnection)
    {
        ConnectionString = connectionString;
        InMemoryKeepAliveConnection = inMemoryKeepAliveConnection;
    }

    public void Dispose()
    {
        InMemoryKeepAliveConnection?.Dispose();
    }
}