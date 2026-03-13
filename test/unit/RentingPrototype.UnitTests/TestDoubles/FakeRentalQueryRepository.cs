using RentingPrototype.Application.Rental.Ports;
using RentingPrototype.Application.Rental.Queries;

namespace RentingPrototype.UnitTests.TestDoubles;

public sealed class FakeRentalQueryRepository : IRentalQueryRepository
{
    public bool HasOpenRentalByCustomer { get; set; }
    public bool HasOpenRentalByVehicle { get; set; }
    public RentalQueryResultDto? RentalByIdResult { get; set; }

    public Task<RentalQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token)
        => Task.FromResult(RentalByIdResult);

    public Task<bool> HasOpenRentalByCustomerAsync(Guid customerId, CancellationToken token)
        => Task.FromResult(HasOpenRentalByCustomer);

    public Task<bool> HasOpenRentalByVehicleAsync(Guid vehicleId, CancellationToken token)
        => Task.FromResult(HasOpenRentalByVehicle);
}
