using System.Data;

namespace RentingPrototype.Infrastructure.Persistence.Sqlite;

public interface ISqliteConnectionFactory
{
    /// <summary>
    /// Creates a new database connection instance.
    /// </summary>
    /// <returns>An unopened SQLite connection.</returns>
    IDbConnection CreateConnection();
}
