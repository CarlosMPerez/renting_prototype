using Dapper;
using RentingPrototype.Application.RentalHistory.Ports;
using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;
using RentingPrototype.Infrastructure.Persistence.Sqlite;

namespace RentingPrototype.Infrastructure.RentalHistory.Adapters;

public class SqliteRentalHistoryQueryRepository : IRentalHistoryQueryRepository
{
    private readonly ISqliteConnectionFactory _factory;

    /// <summary>
    /// Creates a rental history query repository.
    /// </summary>
    /// <param name="factory">SQLite connection factory.</param>
    public SqliteRentalHistoryQueryRepository(ISqliteConnectionFactory factory)
        => _factory = factory;

    /// <summary>
    /// Returns rental history entries for a customer ordered by start date descending.
    /// </summary>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>History rows containing vehicle metadata and rental period.</returns>
    public async Task<IReadOnlyList<CustomerRentalHistoryResultDto>?> GetByCustomerIdAsync(Guid customerId, CancellationToken token)
    {
        const string sql = """
        SELECT
            v.Id                AS VehicleId,
            v.license_plate     AS LicensePlate,
            v.brand             AS Brand,
            v.model             AS Model,
            rh.start_date       AS StartDate,
            rh.end_date         AS EndDate
        FROM vehicles v
        INNER JOIN rental_history rh
            ON rh.vehicle_id = v.id
        WHERE rh.customer_id = @CustomerId
        ORDER BY rh.start_date DESC;
        """;

        using var con = _factory.CreateConnection();
        var rows = await con
            .QueryAsync<CustomerRentalHistoryResultDto>(
                new CommandDefinition(sql, new { CustomerId = customerId.ToString("D") },
                cancellationToken: token));
        return rows.AsList();
    }

    /// <summary>
    /// Returns rental history entries for a vehicle ordered by start date descending.
    /// </summary>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>History rows containing customer metadata and rental period.</returns>
    public async Task<IReadOnlyList<VehicleRentalHistoryResultDto>?> GetByVehicleIdAsync(Guid vehicleId, CancellationToken token)
    {
        const string sql = """
        SELECT
        c.id                AS CustomerId, 
            c.document_id   AS DocumentId,
            c.name          AS Name,
            c.surname       AS Surname,
            rh.start_date   AS StartDate,
            rh.end_date     AS EndDate
        FROM customers c
        INNER JOIN rental_history rh
            ON rh.customer_id = c.id
        WHERE rh.vehicle_id = @VehicleId
        ORDER BY rh.start_date DESC;
        """;

        using var con = _factory.CreateConnection();
        var rows = await con
            .QueryAsync<VehicleRentalHistoryResultDto>(
                new CommandDefinition(sql, new { VehicleId = vehicleId.ToString("D") },
                cancellationToken: token));
        return rows.AsList();
    }
}
