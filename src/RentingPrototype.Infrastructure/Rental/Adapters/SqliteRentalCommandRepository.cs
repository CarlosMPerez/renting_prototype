using Dapper;
using RentingPrototype.Application.Rental.Ports;
using RentingPrototype.Infrastructure.Persistence.SQLite;
using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Infrastructure.Rental.Adapters;

public sealed class SqliteRentalCommandRepository : IRentalCommandRepository
{
    private readonly SqliteUnitOfWork _uow;

    /// <summary>
    /// Creates a rental command repository bound to the active unit of work.
    /// </summary>
    /// <param name="uow">SQLite unit of work.</param>
    public SqliteRentalCommandRepository(SqliteUnitOfWork uow)
    {
        _uow = uow;
    }

    /// <summary>
    /// Inserts a new rental row inside the current unit-of-work transaction.
    /// </summary>
    /// <param name="rental">Rental aggregate to persist.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    public async Task CreateAsync(RentalDomain.Rental rental, CancellationToken token)
    {
        if (_uow.Connection is null || _uow.Transaction is null)
            throw new InvalidOperationException("UnitOfWork has not been started.");

        const string sql = @"
            INSERT INTO rental_history (id, customer_id, vehicle_id, start_date, end_date)
            VALUES
            (@Id, @CustomerId, @VehicleId, @StartDate, NULL);
        ";

        var cmd = new CommandDefinition(sql,
            new
            {
                Id = rental.Id.ToString("D"),
                CustomerId = rental.CustomerId.ToString("D"),
                VehicleId = rental.VehicleId.ToString("D"),
                rental.StartDate
            }, transaction: _uow.Transaction, cancellationToken: token);

        await _uow.Connection.ExecuteAsync(cmd);
    }

    /// <summary>
    /// Updates an existing rental row inside the current unit-of-work transaction.
    /// </summary>
    /// <param name="rental">Rental aggregate with updated values.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    public async Task UpdateAsync(Domain.RentalDomain.Rental rental, CancellationToken token)
    {
        if (_uow.Connection is null || _uow.Transaction is null)
            throw new InvalidOperationException("UnitOfWork has not been started.");

        const string sql = @"
            UPDATE rental_history SET 
                end_date = @EndDate
            WHERE
            id = @Id;
        ";

        var cmd = new CommandDefinition(sql,
            new
            {
                Id = rental.Id.ToString("D"),
                rental.EndDate
            }, transaction: _uow.Transaction, cancellationToken: token);

        await _uow.Connection.ExecuteAsync(cmd);
    }
}
