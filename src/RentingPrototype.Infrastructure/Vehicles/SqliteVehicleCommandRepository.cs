using Dapper;
using RentingPrototype.Application.Vehicles.Ports;
using RentingPrototype.Domain.Vehicles;
using RentingPrototype.Infrastructure.Persistence.SQLite;

namespace RentingPrototype.Infrastructure.Vehicles;

public sealed class SqliteVehicleCommandRepository : IVehicleCommandRepository
{
    private readonly SqliteUnitOfWork _uow;

    public SqliteVehicleCommandRepository(SqliteUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken token)
    {
        if (_uow.Connection is null || _uow.Transaction is null)
            throw new InvalidOperationException("UnitOfWork has not been started.");

        const string sql = @"
            INSERT INTO vehicles (id, license_plate, brand, model, manufacture_date)
            VALUES
            (@Id, @LicensePlate, @Brand, @Model, @ManufactureDate);
        ";

        var cmd = new CommandDefinition(sql,
            new
            {
                Id = vehicle.Id.ToString(),
                vehicle.LicensePlate,
                vehicle.Brand,
                vehicle.Model,
                ManufactureDate = vehicle.ManufactureDateUtc.ToString("O")
            }, transaction: _uow.Transaction, cancellationToken: token);

        await _uow.Connection.ExecuteAsync(cmd);
    }
}
