using System.Data;

namespace RentingPrototype.Infrastructure.Persistence.Sqlite;

public interface ISqliteConnectionFactory
{
    IDbConnection CreateConnection();
}
