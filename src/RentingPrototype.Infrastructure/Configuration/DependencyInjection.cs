using Microsoft.Extensions.DependencyInjection;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Ports;
using RentingPrototype.Application.RentalHistory.Ports;
using RentingPrototype.Application.Vehicle.Ports;
using RentingPrototype.Infrastructure.Configuration;
using RentingPrototype.Infrastructure.DomainEvents;
using RentingPrototype.Infrastructure.Logging;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Rental.Adapters;
using RentingPrototype.Infrastructure.RentalHistory.Adapters;
using RentingPrototype.Infrastructure.Vehicle.Adapters;

namespace RentingPrototype.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        StoragePaths storagePaths)
    {
        services.AddSingleton<ISqliteConnectionFactory>(
            _ => new SqliteConnectionFactory(connectionString));

        services.AddScoped<SqliteUnitOfWork>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqliteUnitOfWork>());

        Directory.CreateDirectory(storagePaths.LogsDirectory);
        var appLogFilePath = storagePaths.ApplicationLogFilePath;

        services.AddSingleton<IAppLogSink>(_ => new TextFileAppLogSink(appLogFilePath));
        services.AddScoped<IDomainEventDispatcher, TextFileDomainEventDispatcher>();

        services.AddScoped<IVehicleCommandRepository, SqliteVehicleCommandRepository>();
        services.AddScoped<IVehicleQueryRepository, SqliteVehicleQueryRepository>();
        services.AddScoped<IRentalCommandRepository, SqliteRentalCommandRepository>();
        services.AddScoped<IRentalQueryRepository, SqliteRentalQueryRepository>();
        services.AddScoped<IRentalHistoryQueryRepository, SqliteRentalHistoryQueryRepository>();

        return services;
    }
}