using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Application.Rental.Ports;

public interface IRentalCommandRepository
{
    /// <summary>
    /// Persists a new rental.
    /// </summary>
    /// <param name="rental">Rental aggregate to persist.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    Task CreateAsync(RentalDomain.Rental rental, CancellationToken token);

    /// <summary>
    /// Persists changes for an existing rental.
    /// </summary>
    /// <param name="rental">Rental aggregate with updated values.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    Task UpdateAsync(RentalDomain.Rental rental, CancellationToken token);
}
