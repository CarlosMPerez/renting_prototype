using RentingPrototype.Application.Rental.Queries;

namespace RentingPrototype.Application.Rental.Interfaces;

public interface IRentalQueryRepository
{
    /// <summary>
    /// Returns a rental by identifier.
    /// </summary>
    /// <param name="id">Rental identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The rental when found; otherwise <c>null</c>.</returns>
    Task<RentalQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token);

    /// <summary>
    /// Checks whether a customer has an active rental.
    /// </summary>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns><c>true</c> when an active rental exists; otherwise <c>false</c>.</returns>
    Task<bool> HasOpenRentalByCustomerAsync(Guid customerId, CancellationToken token);

    /// <summary>
    /// Checks whether a vehicle has an active rental.
    /// </summary>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns><c>true</c> when an active rental exists; otherwise <c>false</c>.</returns>
    Task<bool> HasOpenRentalByVehicleAsync(Guid vehicleId, CancellationToken token);
}
