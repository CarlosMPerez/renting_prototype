using RentingPrototype.Application.Vehicle.Queries;

namespace RentingPrototype.Application.Vehicle.Ports;

public interface IVehicleQueryRepository
{
    /// <summary>
    /// Returns a vehicle by identifier.
    /// </summary>
    /// <param name="id">Vehicle identifier.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The vehicle when found; otherwise <c>null</c>.</returns>
    Task<VehicleQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token);

    /// <summary>
    /// Returns the full list of vehicles.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>All registered vehicles.</returns>
    Task<IReadOnlyList<VehicleQueryResultDto>> GetAllAsync(CancellationToken token);

    /// <summary>
    /// Returns vehicles that are not currently rented.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>Available vehicles.</returns>
    Task<IReadOnlyList<VehicleQueryResultDto>> GetAvailableAsync(CancellationToken token);
}
