using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;
using RentingPrototype.UnitTests.TestDoubles;

namespace RentingPrototype.UnitTests.RentalHistoryTesting;

public sealed class RentalHistoryQueryHandlersTests
{
    [Fact]
    public async Task VehicleHandler_DelegatesToRepository_AndReturnsResult()
    {
        var expected = new List<VehicleRentalHistoryResultDto>
        {
            new(Guid.NewGuid(), "DOC001", "Carlos", "Perez", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)), null)
        };

        var repo = new FakeRentalHistoryQueryRepository { VehicleHistoryResult = expected };
        var handler = new VehicleRentalHistoryQueryHandler(repo);
        var vehicleId = Guid.NewGuid();

        var result = await handler.Handle(new VehicleRentalHistoryFilterDto(vehicleId), CancellationToken.None);

        Assert.Same(expected, result);
        Assert.Equal(vehicleId, repo.LastVehicleId);
    }

    [Fact]
    public async Task CustomerHandler_DelegatesToRepository_AndReturnsResult()
    {
        var expected = new List<CustomerRentalHistoryResultDto>
        {
            new(Guid.NewGuid(), "0001-AAA", "Toyota", "Corolla", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)), null)
        };

        var repo = new FakeRentalHistoryQueryRepository { CustomerHistoryResult = expected };
        var handler = new CustomerRentalHistoryQueryHandler(repo);
        var customerId = Guid.NewGuid();

        var result = await handler.Handle(new CustomerRentalHistoryFilterDto(customerId), CancellationToken.None);

        Assert.Same(expected, result);
        Assert.Equal(customerId, repo.LastCustomerId);
    }
}
