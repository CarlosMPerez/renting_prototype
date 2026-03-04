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
builder.Services.AddScoped<IRentalCommandRepository, SqliteRentalCommandRepository>();
builder.Services.AddScoped<IRentalQueryRepository, SqliteRentalQueryRepository>();


// Handlers scoped
builder.Services.AddScoped<CreateVehicleHandler>();
builder.Services.AddScoped<GetVehicleByIdQueryHandler>();
builder.Services.AddScoped<GetAllVehiclesQueryHandler>();
builder.Services.AddScoped<GetAvailableVehiclesQueryHandler>();

builder.Services.AddScoped<CreateRentalHandler>();
builder.Services.AddScoped<UpdateRentalHandler>();
builder.Services.AddScoped<GetRentalByIdQueryHandler>();



builder.Services.AddOpenApi();

var app = builder.Build();

app.MapVehicleEndpoints();
app.MapRentalsEndpoints();

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

        Console.WriteLine("Database created, seed initial data inserted.");
    }

    return cnnString;
}

public partial class Program { }