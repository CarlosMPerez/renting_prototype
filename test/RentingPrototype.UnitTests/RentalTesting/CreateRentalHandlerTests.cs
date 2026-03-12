using RentingPrototype.Application.Rental.Commands;
using RentingPrototype.Domain.Common.Exceptions;
using RentingPrototype.Domain.RentalDomain.Events;
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
        var dispatcher = new FakeDomainEventDispatcher();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow, dispatcher);

        var cmd = new CreateRentalCommandDto(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1));

        var ex = await Assert.ThrowsAsync<BusinessRuleViolationException>(() =>
            handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None));

        Assert.Contains("Customer already has an active rental", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(uow.Begun);
        Assert.False(commandRepo.Created);
        Assert.Empty(dispatcher.PublishedEvents);
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
        var dispatcher = new FakeDomainEventDispatcher();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow, dispatcher);

        var cmd = new CreateRentalCommandDto(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1));

        var ex = await Assert.ThrowsAsync<BusinessRuleViolationException>(() =>
            handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None));

        Assert.Contains("Vehicle already has an active rental", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.False(uow.Begun);
        Assert.False(commandRepo.Created);
        Assert.Empty(dispatcher.PublishedEvents);
    }

    [Fact]
    public async Task HandleAsync_Begins_Creates_Commits()
    {
        var commandRepo = new FakeRentalCommandRepository();
        var queryRepo = new FakeRentalQueryRepository();
        var uow = new FakeUnitOfWork();
        var dispatcher = new FakeDomainEventDispatcher();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow, dispatcher);

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
        Assert.Single(dispatcher.PublishedEvents);
        Assert.IsType<VehicleRentedDomainEvent>(dispatcher.PublishedEvents[0]);
    }

    [Fact]
    public async Task HandleAsync_WhenCommandRepositoryFails_RollsBack()
    {
        var commandRepo = new FakeRentalCommandRepository { ThrowOnCreate = true };
        var queryRepo = new FakeRentalQueryRepository();
        var uow = new FakeUnitOfWork();
        var dispatcher = new FakeDomainEventDispatcher();
        var handler = new CreateRentalHandler(commandRepo, queryRepo, uow, dispatcher);

        var cmd = new CreateRentalCommandDto(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(cmd, DateTime.UtcNow, CancellationToken.None));

        Assert.True(uow.Begun);
        Assert.True(uow.RolledBack);
        Assert.False(uow.Committed);
        Assert.Empty(dispatcher.PublishedEvents);
    }
}
