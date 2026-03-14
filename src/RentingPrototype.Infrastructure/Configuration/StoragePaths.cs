using Microsoft.AspNetCore.Hosting;

namespace RentingPrototype.Infrastructure.Configuration;

public sealed class StoragePaths
{
    public string ContentRootPath { get; }
    public string DataDirectory { get; }
    public string LogsDirectory { get; }
    public string SchemaDirectory { get; }
    public string DatabaseFilePath { get; }
    public string SchemaFilePath { get; }
    public string ApplicationLogFilePath { get; }

    private StoragePaths(
        string contentRootPath,
        string dataDirectory,
        string logsDirectory,
        string schemaDirectory,
        string databaseFilePath,
        string schemaFilePath,
        string applicationLogFilePath)
    {
        ContentRootPath = contentRootPath;
        DataDirectory = dataDirectory;
        LogsDirectory = logsDirectory;
        SchemaDirectory = schemaDirectory;
        DatabaseFilePath = databaseFilePath;
        SchemaFilePath = schemaFilePath;
        ApplicationLogFilePath = applicationLogFilePath;
    }

    public static StoragePaths From(IWebHostEnvironment environment)
    {
        var contentRootPath = environment.ContentRootPath;

        var dataDirectory = Path.Combine(contentRootPath, "data");
        var logsDirectory = Path.Combine(contentRootPath, "logs");
        var schemaDirectory = Path.Combine(contentRootPath, "data", "schema");

        var databaseFilePath = Path.Combine(dataDirectory, "rentingprototype.db");
        var schemaFilePath = Path.Combine(schemaDirectory, "rentingprototype-schema.sql");
        var applicationLogFilePath = Path.Combine(logsDirectory, "log.txt");

        return new StoragePaths(
            contentRootPath,
            dataDirectory,
            logsDirectory,
            schemaDirectory,
            databaseFilePath,
            schemaFilePath,
            applicationLogFilePath);
    }
}