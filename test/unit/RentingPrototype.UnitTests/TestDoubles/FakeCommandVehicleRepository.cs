using RentingPrototype.Application.Vehicle.Ports;
using RentingPrototype.Domain.VehicleDomain;

namespace RentingPrototype.UnitTests.TestDoubles;

public sealed class FakeCommandVehicleRepository : IVehicleCommandRepository
{
    public bool Added { get; private set; }
    public bool ThrowOnAdd { get; set; }

    public Task AddAsync(Vehicle vehicle, CancellationToken ct)
    {
        if (ThrowOnAdd) throw new InvalidOperationException("DB blew up!!");
        Added = true;
        return Task.CompletedTask;
    }
}