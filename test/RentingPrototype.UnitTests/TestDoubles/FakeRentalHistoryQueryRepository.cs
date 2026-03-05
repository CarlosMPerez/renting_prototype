using RentingPrototype.Application.RentalHistory.Interfaces;
using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;

namespace RentingPrototype.UnitTests.TestDoubles;

public sealed class FakeRentalHistoryQueryRepository : IRentalHistoryQueryRepository
{
    public Guid? LastVehicleId { get; private set; }
    public Guid? LastCustomerId { get; private set; }

    public IReadOnlyList<VehicleRentalHistoryResultDto>? VehicleHistoryResult { get; set; }
    public IReadOnlyList<CustomerRentalHistoryResultDto>? CustomerHistoryResult { get; set; }

    public Task<IReadOnlyList<VehicleRentalHistoryResultDto>?> GetByVehicleIdAsync(Guid vehicleId, CancellationToken token)
    {
        LastVehicleId = vehicleId;
        return Task.FromResult(VehicleHistoryResult);
    }

    public Task<IReadOnlyList<CustomerRentalHistoryResultDto>?> GetByCustomerIdAsync(Guid customerId, CancellationToken token)
    {
        LastCustomerId = customerId;
        return Task.FromResult(CustomerHistoryResult);
    }
}
