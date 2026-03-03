using RentingPrototype.Application.Vehicles.Ports;
using RentingPrototype.Domain.Vehicles;

namespace RentingPrototype.UnitTests.TestDoubles;

public sealed class FakeVehicleRepository : IVehicleCommandRepository
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