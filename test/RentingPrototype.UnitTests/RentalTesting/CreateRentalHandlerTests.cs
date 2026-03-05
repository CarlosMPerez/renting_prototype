using RentingPrototype.Application.Rental.Commands;
using RentingPrototype.UnitTests.TestDoubles;

namespace RentingPrototype.UnitTests.RentalTesting;

public sealed class CreateRentalHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenCustomerHasOpenRental_Throws_AndDoesNotStartTransaction()
    {
        var commandRepo = new FakeRentalCommandRepository();
        var queryRepo = new FakeRentalQueryRepository
        {
            HasOpenRentalByCustomer = true,
            HasOpenRentalByVehicle = false
        };
        var uow = new FakeUnitOfWork();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow);

        var cmd = new CreateRentalCommandDto(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None));

        Assert.Contains("Customer already has an active rental", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(uow.Begun);
        Assert.False(commandRepo.Created);
    }

    [Fact]
    public async Task HandleAsync_WhenVehicleHasOpenRental_Throws_AndDoesNotStartTransaction()
    {
        var commandRepo = new FakeRentalCommandRepository();
        var queryRepo = new FakeRentalQueryRepository
        {
            HasOpenRentalByCustomer = false,
            HasOpenRentalByVehicle = true
        };
        var uow = new FakeUnitOfWork();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow);

        var cmd = new CreateRentalCommandDto(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None));

        Assert.Contains("Vehicle already has an active rental", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(uow.Begun);
        Assert.False(commandRepo.Created);
    }

    [Fact]
    public async Task HandleAsync_Begins_Creates_Commits()
    {
        var commandRepo = new FakeRentalCommandRepository();
        var queryRepo = new FakeRentalQueryRepository();
        var uow = new FakeUnitOfWork();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow);

        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var cmd = new CreateRentalCommandDto(customerId, vehicleId, DateTime.UtcNow.AddDays(-1));

        var result = await handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None);

        Assert.True(uow.Begun);
        Assert.True(commandRepo.Created);
        Assert.True(uow.Committed);
        Assert.False(uow.RolledBack);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(customerId, commandRepo.LastCreatedRental!.CustomerId);
        Assert.Equal(vehicleId, commandRepo.LastCreatedRental.VehicleId);
    }

    [Fact]
    public async Task HandleAsync_WhenCommandRepositoryFails_RollsBack()
    {
        var commandRepo = new FakeRentalCommandRepository { ThrowOnCreate = true };
        var queryRepo = new FakeRentalQueryRepository();
        var uow = new FakeUnitOfWork();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow);

        var cmd = new CreateRentalCommandDto(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None));

        Assert.True(uow.Begun);
        Assert.True(uow.RolledBack);
        Assert.False(uow.Committed);
    }
}
