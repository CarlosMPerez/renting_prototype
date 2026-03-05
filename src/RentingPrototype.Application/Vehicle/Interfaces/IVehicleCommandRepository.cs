using VehicleDomain = RentingPrototype.Domain.VehicleDomain;

namespace RentingPrototype.Application.Vehicle.Interfaces;

public interface IVehicleCommandRepository
{
    /// <summary>
    /// Persists a new vehicle entity.
    /// </summary>
    /// <param name="vehicle">Vehicle aggregate to persist.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    Task AddAsync(VehicleDomain.Vehicle vehicle, CancellationToken token);
}
