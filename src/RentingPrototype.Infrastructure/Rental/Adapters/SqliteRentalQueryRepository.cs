using Dapper;
using RentingPrototype.Application.Rental.Ports;
using RentingPrototype.Application.Rental.Queries;
using RentingPrototype.Infrastructure.Persistence.Sqlite;

namespace RentingPrototype.Infrastructure.Rental.Adapters;

public sealed class SqliteRentalQueryRepository : IRentalQueryRepository
{
    private readonly ISqliteConnectionFactory _factory;

    /// <summary>
    /// Creates a rental query repository.
    /// </summary>
    /// <param name="factory">SQLite connection factory.</param>
    public SqliteRentalQueryRepository(ISqliteConnectionFactory factory)
        => _factory = factory;

    /// <summary>
    /// Returns a rental by identifier.
    /// </summary>
    /// <param name="id">Rental identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The rental when found; otherwise <c>null</c>.</returns>
    public async Task<RentalQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token)
    {
        const string sql = """
        SELECT
            rh.id            AS Id,
            rh.customer_id   AS CustomerId,
            rh.vehicle_id    AS VehicleId,
            rh.start_date    AS StartDate,
            rh.end_date      AS EndDate
        FROM rental_history rh
        WHERE rh.id = @Id
        LIMIT 1;
        """;

        using var con = _factory.CreateConnection();
        return await con
            .QuerySingleOrDefaultAsync<RentalQueryResultDto>(
                new CommandDefinition(sql, new { Id = id.ToString("D") }, cancellationToken: token));
    }

    /// <summary>
    /// Checks whether a customer has an active rental.
    /// </summary>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns><c>true</c> if an active rental exists; otherwise <c>false</c>.</returns>
    public async Task<bool> HasOpenRentalByCustomerAsync(Guid customerId, CancellationToken token)
    {
        const string sql = """
        SELECT 1
        FROM rental_history
        WHERE customer_id = @CustomerId
        AND end_date IS NULL
        LIMIT 1;
        """;

        using var con = _factory.CreateConnection();
        var exists = await con.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(sql, new { CustomerId = customerId.ToString("D") }, cancellationToken: token));

        return exists.HasValue;
    }

    /// <summary>
    /// Checks whether a vehicle has an active rental.
    /// </summary>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns><c>true</c> if an active rental exists; otherwise <c>false</c>.</returns>
    public async Task<bool> HasOpenRentalByVehicleAsync(Guid vehicleId, CancellationToken token)
    {
        const string sql = """
        SELECT 1
        FROM rental_history
        WHERE vehicle_id = @VehicleId
        AND end_date IS NULL
        LIMIT 1;
        """;

        using var con = _factory.CreateConnection();
        var exists = await con.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(sql, new { VehicleId = vehicleId.ToString("D") }, cancellationToken: token));

        return exists.HasValue;
    }
}
