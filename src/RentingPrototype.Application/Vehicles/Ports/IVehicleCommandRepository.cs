using RentingPrototype.Domain.Vehicles;

namespace RentingPrototype.Application.Vehicles.Ports;

public interface IVehicleCommandRepository
{
    Task AddAsync(Vehicle vehicle, CancellationToken token);
}
