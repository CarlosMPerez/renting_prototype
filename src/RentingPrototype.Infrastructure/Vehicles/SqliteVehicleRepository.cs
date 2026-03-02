using Dapper;
using RentingPrototype.Application.Vehicles.Ports;
using RentingPrototype.Domain.Vehicles;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Persistence.SQLite;

namespace RentingPrototype.Infrastructure.Vehicles;

public sealed class SqliteVehicleRepository : IVehicleRepository
{
    private readonly SqliteUnitOfWork _uow;

    public SqliteVehicleRepository(SqliteUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken token)
    {
        if (_uow.Connection is null || _uow.Transaction is null)
            throw new InvalidOperationException("UnitOfWork has not been started.");
        
        const string sql = @"
            INSERT INTO vehicles (id, license_plate, make, model, manufacturing_date)
            VALUES
            (@Id, @LicensePlate, @Make, @Model, @ManufacturingDate);
        ";

        var cmd = new CommandDefinition(sql,
            new
            {
                Id = vehicle.Id.ToString(),
                vehicle.LicensePlate,
                vehicle.Make,
                vehicle.Model,
                ManufacturingDate = vehicle.ManufacturingDateUtc.ToString("O")
            }, transaction: _uow.Transaction, cancellationToken: token);
        
        await _uow.Connection.ExecuteAsync(cmd);
    }
}
