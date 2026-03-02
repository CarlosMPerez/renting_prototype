using RentingPrototype.Domain.Vehicles;

namespace RentingPrototype.Application.Vehicles.Ports;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle, CancellationToken token);
}
