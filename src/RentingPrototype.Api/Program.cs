using Microsoft.Data.Sqlite;
using RentingPrototype.Api.Endpoints.Vehicles;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicles;
using RentingPrototype.Application.Vehicles.Ports;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Persistence.SQLite;
using RentingPrototype.Infrastructure.Vehicles;

var builder = WebApplication.CreateBuilder(args);

// Create minimal DB schema
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

// Registering services
builder.Services.AddSingleton<ISqliteConnectionFactory>(_ => new SqliteConnectionFactory(cnnString));

// UoW como Scoped (por request)
builder.Services.AddScoped<SqliteUnitOfWork>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqliteUnitOfWork>());

// Repo scoped
builder.Services.AddScoped<IVehicleRepository, SqliteVehicleRepository>();

// Handler scoped
builder.Services.AddScoped<CreateVehicleHandler>();

var app = builder.Build();

app.MapVehicleEndpoints();

app.Run();
