using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;

namespace RentingPrototype.Application.RentalHistory.Interfaces;

public interface IRentalHistoryQueryRepository
{
    /// <summary>
    /// Returns the rental history for a vehicle.
    /// </summary>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>Vehicle rental history entries including customer data.</returns>
    Task<IReadOnlyList<VehicleRentalHistoryResultDto>?> GetByVehicleIdAsync(Guid vehicleId, CancellationToken token);

    /// <summary>
    /// Returns the rental history for a customer.
    /// </summary>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>Customer rental history entries including vehicle data.</returns>
    Task<IReadOnlyList<CustomerRentalHistoryResultDto>?> GetByCustomerIdAsync(Guid customerId, CancellationToken token);
}
