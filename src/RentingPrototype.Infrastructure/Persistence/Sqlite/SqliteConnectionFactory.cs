using System.Data;
using Microsoft.Data.Sqlite;

namespace RentingPrototype.Infrastructure.Persistence.Sqlite;

public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly string _cnnString;

    public SqliteConnectionFactory(string connectionString)
    {
        _cnnString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqliteConnection(_cnnString);
}
