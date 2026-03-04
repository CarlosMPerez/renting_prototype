using VehicleDomain = RentingPrototype.Domain.VehicleDomain;

namespace RentingPrototype.Application.Vehicle.Interfaces;

public interface IVehicleCommandRepository
{
    Task AddAsync(VehicleDomain.Vehicle vehicle, CancellationToken token);
}
