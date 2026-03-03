using RentingPrototype.Application.Vehicles;
using RentingPrototype.Application.Vehicles.CreateVehicle;
using RentingPrototype.UnitTests.TestDoubles;

namespace RentingPrototype.UnitTests.VehicleTesting;

public sealed class CreateVehicleHandlerTests
{
    [Fact]
    public async Task HandleAsync_Begins_Adds_Commits()
    {
        var repo = new FakeVehicleRepository();
        var uow = new FakeUnitOfWork();
        var handler = new CreateVehicleHandler(repo, uow);

        var cmd = new CreateVehicleCommand("1234-ABC", "Toyota", "Corolla", DateTime.UtcNow.AddYears(-3));
        var result = await handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None);

        Assert.True(uow.Begun);
        Assert.True(repo.Added);
        Assert.True(uow.Committed);
        Assert.False(uow.RolledBack);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task HandleAsync_WhenRepoFails_RollsBack()
    {
        var repo = new FakeVehicleRepository { ThrowOnAdd = true };
        var uow = new FakeUnitOfWork();
        var handler = new CreateVehicleHandler(repo, uow);

        var cmd = new CreateVehicleCommand("1234-ABC", "Toyota", "Corolla", DateTime.UtcNow.AddYears(-3));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None));

        Assert.True(uow.Begun);
        Assert.True(uow.RolledBack);
        Assert.False(uow.Committed);
    }
}