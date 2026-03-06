using Microsoft.Data.Sqlite;
using Scalar.AspNetCore;
using RentingPrototype.Api.Endpoints.Vehicle;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Commands;
using RentingPrototype.Application.Vehicle.Interfaces;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Persistence.SQLite;
using RentingPrototype.Infrastructure.Vehicle;
using RentingPrototype.Application.Vehicle.Queries;
using RentingPrototype.Api.Endpoints.Rental;
using RentingPrototype.Infrastructure.Rental;
using RentingPrototype.Application.Rental.Interfaces;
using RentingPrototype.Application.Rental.Commands;
using RentingPrototype.Application.Rental.Queries;
using RentingPrototype.Api.Endpoints.RentalHistory;
using RentingPrototype.Application.RentalHistory.Interfaces;
using RentingPrototype.Infrastructure.RentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;

var builder = WebApplication.CreateBuilder(args);

// Create minimal DB schema
var (connectionString, inMemoryKeepAliveConnection) = CreateMinimalDbSchema();

// Registering services
builder.Services.AddSingleton<ISqliteConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

// UoW como Scoped (por request)
builder.Services.AddScoped<SqliteUnitOfWork>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqliteUnitOfWork>());

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

var app = builder.Build();

if (inMemoryKeepAliveConnection is not null)
{
    app.Lifetime.ApplicationStopped.Register(() => inMemoryKeepAliveConnection.Dispose());
}

app.MapVehicleEndpoints();
app.MapRentalsEndpoints();
app.MapRentalHistoryEndpoints();

app.MapOpenApi("/openapi/{documentName}.json");
app.MapScalarApiReference(options =>
{
    options.WithTitle("Renting Prototype API");
});

app.Run();

/// <summary>
/// Creates the SQLite schema required by the application and returns the connection data.
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

    var dataDir = Path.Combine(builder.Environment.ContentRootPath, ".data");
    Directory.CreateDirectory(dataDir);
    var dbPath = Path.Combine(dataDir, "rentingprototype.db");

    var cnnString = new SqliteConnectionStringBuilder
    {
        DataSource = dbPath
    }.ToString();

    if (!File.Exists(dbPath))
    {
        Console.WriteLine("Database not found. Creating new SQLite database...");
        using var conn = new SqliteConnection(cnnString);
        conn.Open();

        using var command = conn.CreateCommand();
        command.CommandText = schemaSql;
        command.ExecuteNonQuery();

        Console.WriteLine("Database created, seed initial data inserted.");
    }

    return (cnnString, null);
}

public partial class Program { }
