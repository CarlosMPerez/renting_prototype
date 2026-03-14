using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using RentingPrototype.Infrastructure.Configuration;

namespace RentingPrototype.Infrastructure.Persistence.Sqlite;

public static class SqliteDatabaseInitializer
{
    public static SqliteDatabaseInitializationResult Initialize(
        IWebHostEnvironment environment,
        StoragePaths storagePaths)
    {
        if (environment.IsEnvironment("Testing"))
        {
            var inMemoryConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = $"rentingprototype-tests-{Guid.NewGuid():N}",
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            var keepAliveConnection = new SqliteConnection(inMemoryConnectionString);
            keepAliveConnection.Open();

            CreateMinimalDbSchema(keepAliveConnection, storagePaths.SchemaFilePath);

            return new SqliteDatabaseInitializationResult(
                inMemoryConnectionString,
                keepAliveConnection);
        }

        Directory.CreateDirectory(storagePaths.DataDirectory);

        var connectionString = $"Data Source={storagePaths.DatabaseFilePath}";

        if (!File.Exists(storagePaths.DatabaseFilePath))
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            CreateMinimalDbSchema(connection, storagePaths.SchemaFilePath);
        }

        return new SqliteDatabaseInitializationResult(
            connectionString,
            null);
    }

    private static void CreateMinimalDbSchema(
        SqliteConnection connection,
        string schemaFilePath)
    {
        if (!File.Exists(schemaFilePath))
        {
            throw new FileNotFoundException(
                $"Sqlite schema file was not found at '{schemaFilePath}'.",
                schemaFilePath);
        }

        var sql = File.ReadAllText(schemaFilePath);
        Console.WriteLine("Executing database schema and inserting seed data");
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
