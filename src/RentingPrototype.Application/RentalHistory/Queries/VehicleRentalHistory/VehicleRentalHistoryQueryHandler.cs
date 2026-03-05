using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.RentalHistory.Interfaces;

namespace RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;

public sealed record VehicleRentalHistoryFilterDto(Guid vehicleId);
public sealed class VehicleRentalHistoryQueryHandler : IQueryHandler<VehicleRentalHistoryFilterDto, IReadOnlyList<VehicleRentalHistoryResultDto>>
{
    private readonly IRentalHistoryQueryRepository _repo;

    /// <summary>
    /// Creates a handler instance for vehicle rental history queries.
    /// </summary>
    /// <param name="repo">Rental history query repository.</param>
    public VehicleRentalHistoryQueryHandler(IRentalHistoryQueryRepository repo) => _repo = repo;

    /// <summary>
    /// Retrieves rental history entries for a vehicle.
    /// </summary>
    /// <param name="query">Vehicle rental history filter.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>History rows containing customer and rental period data.</returns>
    public Task<IReadOnlyList<VehicleRentalHistoryResultDto>?> Handle(VehicleRentalHistoryFilterDto query, CancellationToken token) 
        => _repo.GetByVehicleIdAsync(query.vehicleId, token);
}
