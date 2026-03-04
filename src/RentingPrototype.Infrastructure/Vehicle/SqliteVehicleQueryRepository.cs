using Dapper;
using RentingPrototype.Application.Vehicle.Interfaces;
using RentingPrototype.Application.Vehicle.Queries;
using RentingPrototype.Infrastructure.Persistence.Sqlite;

namespace RentingPrototype.Infrastructure.Vehicle;

public sealed class SqliteVehicleQueryRepository : IVehicleQueryRepository
{
    private readonly ISqliteConnectionFactory _factory;

    public SqliteVehicleQueryRepository(ISqliteConnectionFactory factory)
        => _factory = factory;

    public async Task<IReadOnlyList<VehicleQueryResultDto>> GetAllAsync(CancellationToken token)
    {
        const string sql = """
        SELECT
            v.id              AS Id,
            v.license_plate   AS LicensePlate,
            v.brand           AS Brand,
            v.model           AS Model,
            v.manufacture_date AS ManufactureDate
        FROM vehicles v;
        """;

        using var con = _factory.CreateConnection();
        var rows = await con
            .QueryAsync<VehicleQueryResultDto>(
                new CommandDefinition(sql, cancellationToken: token));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<VehicleQueryResultDto>> GetAvailableAsync(CancellationToken token)
    {
        // "Disponibles" = NO tienen un renting_history activo (end_date IS NULL)
        const string sql = """
        SELECT
            v.id              AS Id,
            v.license_plate   AS LicensePlate,
            v.brand           AS Brand,
            v.model           AS Model,
            v.manufacture_date AS ManufactureDate
        FROM vehicles v
        LEFT JOIN rental_history rh
            ON rh.vehicle_id = v.id
           AND rh.end_date IS NULL
        WHERE rh.vehicle_id IS NULL;
        """;

        using var con = _factory.CreateConnection();
        var rows = await con
            .QueryAsync<VehicleQueryResultDto>(
                new CommandDefinition(sql, cancellationToken: token));
        return rows.AsList();
    }

    public async Task<VehicleQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token)
    {
        const string sql = """
        SELECT
            v.id              AS Id,
            v.license_plate   AS LicensePlate,
            v.brand           AS Brand,
            v.model           AS Model,
            v.manufacture_date AS ManufactureDate
        FROM vehicles v
        WHERE v.id = @Id
        LIMIT 1;
        """;

        using var con = _factory.CreateConnection();
        return await con
            .QuerySingleOrDefaultAsync<VehicleQueryResultDto>(
                new CommandDefinition(sql, new { Id = id.ToString("D") }, cancellationToken: token));
    }
}