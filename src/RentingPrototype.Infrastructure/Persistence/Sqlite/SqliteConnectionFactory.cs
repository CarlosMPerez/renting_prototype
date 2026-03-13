using System.Data;
using Microsoft.Data.Sqlite;

namespace RentingPrototype.Infrastructure.Persistence.Sqlite;

public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly string _cnnString;

    /// <summary>
    /// Creates a Sqlite connection factory.
    /// </summary>
    /// <param name="connectionString">Sqlite connection string.</param>
    public SqliteConnectionFactory(string connectionString)
    {
        _cnnString = connectionString;
    }

    /// <summary>
    /// Creates a new Sqlite connection configured with the factory connection string.
    /// </summary>
    /// <returns>An unopened Sqlite connection.</returns>
    public IDbConnection CreateConnection() => new SqliteConnection(_cnnString);
}
