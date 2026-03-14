using Scalar.AspNetCore;
using RentingPrototype.Api.Endpoints;
using RentingPrototype.Application.Configuration;
using RentingPrototype.Host.ExceptionHandling;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Configuration;
using RentingPrototype.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var storagePaths = StoragePaths.From(builder.Environment);
// Create minimal DB schema
var databaseInit = SqliteDatabaseInitializer.Initialize(builder.Environment, storagePaths);
var connectionString = databaseInit.ConnectionString;

// Registering services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(databaseInit.ConnectionString, storagePaths);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.Lifetime.ApplicationStopped.Register(databaseInit.Dispose);

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


public partial class Program { }
