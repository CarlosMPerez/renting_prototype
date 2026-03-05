using Dapper;
using RentingPrototype.Application.Vehicle.Interfaces;
using VehicleDomain = RentingPrototype.Domain.VehicleDomain;
using RentingPrototype.Infrastructure.Persistence.SQLite;

namespace RentingPrototype.Infrastructure.Vehicle;

public sealed class SqliteVehicleCommandRepository : IVehicleCommandRepository
{
    private readonly SqliteUnitOfWork _uow;

    /// <summary>
    /// Creates a vehicle command repository bound to the active unit of work.
    /// </summary>
    /// <param name="uow">SQLite unit of work.</param>
    public SqliteVehicleCommandRepository(SqliteUnitOfWork uow)
    {
        _uow = uow;
    }

    /// <summary>
    /// Inserts a new vehicle row inside the current unit-of-work transaction.
    /// </summary>
    /// <param name="vehicle">Vehicle aggregate to persist.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    public async Task AddAsync(VehicleDomain.Vehicle vehicle, CancellationToken token)
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
                Id = vehicle.Id.ToString("D"),
                vehicle.LicensePlate,
                vehicle.Brand,
                vehicle.Model,
                ManufactureDate = vehicle.ManufactureDateUtc.ToString("O")
            }, transaction: _uow.Transaction, cancellationToken: token);

        await _uow.Connection.ExecuteAsync(cmd);
    }
}
