using Dapper;
using RentingPrototype.Application.Vehicle.Ports;
using RentingPrototype.Application.Vehicle.Queries;
using RentingPrototype.Infrastructure.Persistence.Sqlite;

namespace RentingPrototype.Infrastructure.Vehicle.Adapters;

public sealed class SqliteVehicleQueryRepository : IVehicleQueryRepository
{
    private readonly ISqliteConnectionFactory _factory;

    /// <summary>
    /// Creates a vehicle query repository.
    /// </summary>
    /// <param name="factory">SQLite connection factory.</param>
    public SqliteVehicleQueryRepository(ISqliteConnectionFactory factory)
        => _factory = factory;

    /// <summary>
    /// Returns all vehicles from persistence.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>All vehicle rows.</returns>
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

    /// <summary>
    /// Returns vehicles with no active rental.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>Available vehicles.</returns>
    public async Task<IReadOnlyList<VehicleQueryResultDto>> GetAvailableAsync(CancellationToken token)
    {
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

    /// <summary>
    /// Returns a vehicle by identifier.
    /// </summary>
    /// <param name="id">Vehicle identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The vehicle when found; otherwise <c>null</c>.</returns>
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
