using System.Data;
using Microsoft.Data.Sqlite;

namespace RentingPrototype.Infrastructure.Persistence.Sqlite;

public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly string _cnnString;

    /// <summary>
    /// Creates a SQLite connection factory.
    /// </summary>
    /// <param name="connectionString">SQLite connection string.</param>
    public SqliteConnectionFactory(string connectionString)
    {
        _cnnString = connectionString;
    }

    /// <summary>
    /// Creates a new SQLite connection configured with the factory connection string.
    /// </summary>
    /// <returns>An unopened SQLite connection.</returns>
    public IDbConnection CreateConnection() => new SqliteConnection(_cnnString);
}
