using Microsoft.Data.Sqlite;
using Scalar.AspNetCore;
using RentingPrototype.Api.Endpoints.Vehicles;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicles.Commands;
using RentingPrototype.Application.Vehicles.Ports;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Persistence.SQLite;
using RentingPrototype.Infrastructure.Vehicles;
using RentingPrototype.Application.Vehicles.Queries;

var builder = WebApplication.CreateBuilder(args);

// Create minimal DB schema
string connectionString = CreateMinimalDbSchema();

// Registering services
builder.Services.AddSingleton<ISqliteConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

// UoW como Scoped (por request)
builder.Services.AddScoped<SqliteUnitOfWork>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqliteUnitOfWork>());

// Repos scoped
builder.Services.AddScoped<IVehicleCommandRepository, SqliteVehicleCommandRepository>();
builder.Services.AddScoped<IVehicleQueryRepository, SqliteVehicleQueryRepository>();

// Handlers scoped
builder.Services.AddScoped<CreateVehicleHandler>();
builder.Services.AddScoped<GetVehicleByIdQueryHandler>();
builder.Services.AddScoped<GetAllVehiclesHandler>();
builder.Services.AddScoped<GetAvailableVehiclesHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapVehicleEndpoints();

app.MapOpenApi("/openapi/{documentname].json}");
app.MapScalarApiReference(options =>
{
    options.WithTitle("Renting Prototype API");
});

app.Run();

/// Creamos un esquema mínimo de base de datos
/// siguiendo el esquema especificado en rentingprototype-schema.sql
string CreateMinimalDbSchema()
{
    string dbPath;
    if (builder.Environment.IsEnvironment("Testing"))
    {
        dbPath = Path.Combine(Path.GetTempPath(), $"rentingprototype-{Guid.NewGuid():N}.db");
    }
    else
    {
        var dataDir = Path.Combine(builder.Environment.ContentRootPath, ".data");
        Directory.CreateDirectory(dataDir);
        dbPath = Path.Combine(dataDir, "rentingprototype.db");
    }

    var cnnString = new SqliteConnectionStringBuilder
    {
        DataSource = dbPath
    }.ToString();

    if (!File.Exists(dbPath))
    {
        Console.WriteLine("Database not found. Creating new SQLite database...");
        var schemaFile = Path.Combine(AppContext.BaseDirectory,
            "rentingprototype-schema.sql");
        if (!File.Exists(schemaFile)) throw new FileNotFoundException($"Cannot create database. Schema not found at {schemaFile}");

        var schemaSql = File.ReadAllText(schemaFile);
        using var conn = new SqliteConnection(cnnString);
        conn.Open();

        using var command = conn.CreateCommand();
        command.CommandText = schemaSql;
        command.ExecuteNonQuery();

        Console.WriteLine("Database created and schema applied.");
    }

    return cnnString;
}

public partial class Program { }