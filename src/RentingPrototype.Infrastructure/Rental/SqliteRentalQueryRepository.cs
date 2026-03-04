using Dapper;
using RentingPrototype.Application.Rental.Interfaces;
using RentingPrototype.Application.Rental.Queries;
using RentingPrototype.Infrastructure.Persistence.Sqlite;

namespace RentingPrototype.Infrastructure.Rental;

public sealed class SqliteRentalQueryRepository : IRentalQueryRepository
{
    private readonly ISqliteConnectionFactory _factory;

    public SqliteRentalQueryRepository(ISqliteConnectionFactory factory)
        => _factory = factory;

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
}