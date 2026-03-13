using Microsoft.Data.Sqlite;
using Scalar.AspNetCore;
using RentingPrototype.Host.ExceptionHandling;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Commands;
using RentingPrototype.Application.Vehicle.Ports;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Vehicle.Adapters;
using RentingPrototype.Application.Vehicle.Queries;
using RentingPrototype.Infrastructure.Rental.Adapters;
using RentingPrototype.Application.Rental.Ports;
using RentingPrototype.Application.Rental.Commands;
using RentingPrototype.Application.Rental.Queries;
using RentingPrototype.Application.RentalHistory.Ports;
using RentingPrototype.Infrastructure.RentalHistory.Adapters;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Infrastructure.DomainEvents;
using RentingPrototype.Infrastructure.Logging;
using RentingPrototype.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Create minimal DB schema
var (connectionString, inMemoryKeepAliveConnection) = CreateMinimalDbSchema();

// Registering services
builder.Services.AddSingleton<ISqliteConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

// UoW como Scoped (por request)
builder.Services.AddScoped<SqliteUnitOfWork>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqliteUnitOfWork>());

var logsDirectory = Path.Combine(builder.Environment.ContentRootPath, "logs");
Directory.CreateDirectory(logsDirectory);
var appLogFilePath = Path.Combine(logsDirectory, "log.txt");

builder.Services.AddSingleton<IAppLogSink>(_ => new TextFileAppLogSink(appLogFilePath));
builder.Services.AddScoped<IDomainEventDispatcher, TextFileDomainEventDispatcher>();

// Repos scoped
builder.Services.AddScoped<IVehicleCommandRepository, SqliteVehicleCommandRepository>();
builder.Services.AddScoped<IVehicleQueryRepository, SqliteVehicleQueryRepository>();
builder.Services.AddScoped<IRentalCommandRepository, SqliteRentalCommandRepository>();
builder.Services.AddScoped<IRentalQueryRepository, SqliteRentalQueryRepository>();
builder.Services.AddScoped<IRentalHistoryQueryRepository, SqliteRentalHistoryQueryRepository>();

// Handlers scoped
builder.Services.AddScoped<CreateVehicleHandler>();
builder.Services.AddScoped<GetVehicleByIdQueryHandler>();
builder.Services.AddScoped<GetAllVehiclesQueryHandler>();
builder.Services.AddScoped<GetAvailableVehiclesQueryHandler>();

builder.Services.AddScoped<CreateRentalHandler>();
builder.Services.AddScoped<UpdateRentalHandler>();
builder.Services.AddScoped<GetRentalByIdQueryHandler>();

builder.Services.AddScoped<VehicleRentalHistoryQueryHandler>();
builder.Services.AddScoped<CustomerRentalHistoryQueryHandler>();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

if (inMemoryKeepAliveConnection is not null)
{
    app.Lifetime.ApplicationStopped.Register(() => inMemoryKeepAliveConnection.Dispose());
}

app.UseExceptionHandler();

app.MapApiEndpoints();

app.MapOpenApi("/openapi/{documentName}.json");
app.MapScalarApiReference(options =>
{
    options.WithTitle("Renting Prototype API");
});

// Redirect root to Scalar API ref.
app.MapGet("/", () => Results.Redirect("/scalar"));

app.Run();

/// <summary>
/// Creates the Sqlite schema required by the application and returns the connection data.
/// In testing environment this uses an in-memory database with a keep-alive connection.
/// </summary>
/// <returns>
/// A tuple containing the connection string and an optional keep-alive connection.
/// </returns>
(string ConnectionString, SqliteConnection? InMemoryKeepAliveConnection) CreateMinimalDbSchema()
{
    var schemaFile = Path.Combine(AppContext.BaseDirectory, "rentingprototype-schema.sql");
    if (!File.Exists(schemaFile))
        throw new FileNotFoundException($"Cannot create database. Schema not found at {schemaFile}");

    var schemaSql = File.ReadAllText(schemaFile);

    if (builder.Environment.IsEnvironment("Testing"))
    {
        var inMemoryConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = $"rentingprototype-tests-{Guid.NewGuid():N}",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        }.ToString();

        var keepAliveConnection = new SqliteConnection(inMemoryConnectionString);
        keepAliveConnection.Open();

        using (var command = keepAliveConnection.CreateCommand())
        {
            command.CommandText = schemaSql;
            command.ExecuteNonQuery();
        }

        return (inMemoryConnectionString, keepAliveConnection);
    }

    var dataDir = Path.Combine(builder.Environment.ContentRootPath, "data");
    Directory.CreateDirectory(dataDir);
    var dbPath = Path.Combine(dataDir, "rentingprototype.db");

    var cnnString = new SqliteConnectionStringBuilder
    {
        DataSource = dbPath
    }.ToString();

    if (!File.Exists(dbPath))
    {
        Console.WriteLine("Database not found. Creating new Sqlite database...");
        using var conn = new SqliteConnection(cnnString);
        conn.Open();

        using var command = conn.CreateCommand();
        command.CommandText = schemaSql;
        command.ExecuteNonQuery();

        Console.WriteLine("Database created, seed initial data inserted.");
    }

    Console.WriteLine(cnnString);

    return (cnnString, null);
}

public partial class Program { }
