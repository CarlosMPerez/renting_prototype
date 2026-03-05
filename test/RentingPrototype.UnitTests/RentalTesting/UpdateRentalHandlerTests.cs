using RentingPrototype.Application.Rental.Commands;
using RentingPrototype.Application.Rental.Queries;
using RentingPrototype.UnitTests.TestDoubles;

namespace RentingPrototype.UnitTests.RentalTesting;

public sealed class UpdateRentalHandlerTests
{
    [Fact]
    public async Task HandleAsync_Begins_Updates_Commits()
    {
        var commandRepo = new FakeRentalCommandRepository();
        var queryRepo = new FakeRentalQueryRepository
        {
            RentalByIdResult = new RentalQueryResultDto(
                Id: Guid.NewGuid(),
                CustomerId: Guid.NewGuid(),
                VehicleId: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(-3),
                EndDate: null)
        };
        var uow = new FakeUnitOfWork();
        var handler = new UpdateRentalHandler(commandRepo, queryRepo, uow);

        var rentalId = queryRepo.RentalByIdResult.Id;
        var endDate = DateTime.UtcNow.AddDays(-1);
        var cmd = new UpdateRentalCommandDto(rentalId, endDate);

        var result = await handler.HandleAsync(cmd, CancellationToken.None);

        Assert.True(uow.Begun);
        Assert.True(commandRepo.Updated);
        Assert.True(uow.Committed);
        Assert.False(uow.RolledBack);
        Assert.Equal(rentalId, result.Id);
        Assert.Equal(endDate.Date, commandRepo.LastUpdatedRental!.EndDate!.Value.Date);
    }

    [Fact]
    public async Task HandleAsync_WhenRentalNotFound_RollsBackAndThrows()
    {
        var commandRepo = new FakeRentalCommandRepository();
        var queryRepo = new FakeRentalQueryRepository
        {
            RentalByIdResult = null
        };
        var uow = new FakeUnitOfWork();
        var handler = new UpdateRentalHandler(commandRepo, queryRepo, uow);

        var cmd = new UpdateRentalCommandDto(Guid.NewGuid(), DateTime.UtcNow);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.HandleAsync(cmd, CancellationToken.None));

        Assert.True(uow.Begun);
        Assert.True(uow.RolledBack);
        Assert.False(uow.Committed);
        Assert.False(commandRepo.Updated);
    }

    [Fact]
    public async Task HandleAsync_WhenCommandRepositoryFails_RollsBack()
    {
        var commandRepo = new FakeRentalCommandRepository { ThrowOnUpdate = true };
        var queryRepo = new FakeRentalQueryRepository
        {
            RentalByIdResult = new RentalQueryResultDto(
                Id: Guid.NewGuid(),
                CustomerId: Guid.NewGuid(),
                VehicleId: Guid.NewGuid(),
                StartDate: DateTime.UtcNow.AddDays(-3),
                EndDate: null)
        };
        var uow = new FakeUnitOfWork();
        var handler = new UpdateRentalHandler(commandRepo, queryRepo, uow);

        var cmd = new UpdateRentalCommandDto(queryRepo.RentalByIdResult.Id, DateTime.UtcNow);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(cmd, CancellationToken.None));

        Assert.True(uow.Begun);
        Assert.True(uow.RolledBack);
        Assert.False(uow.Committed);
    }
}
