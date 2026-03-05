using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Interfaces;

namespace RentingPrototype.Application.Vehicle.Queries;

public sealed record ListAvailableVehiclesQueryDto;

public class GetAvailableVehiclesQueryHandler : IQueryHandler<ListVehiclesQueryDto, IReadOnlyList<VehicleQueryResultDto>>
{
    private readonly IVehicleQueryRepository _repo;

    /// <summary>
    /// Creates a handler instance for listing available vehicles.
    /// </summary>
    /// <param name="repo">Vehicle query repository.</param>
    public GetAvailableVehiclesQueryHandler(IVehicleQueryRepository repo)
        => _repo = repo;

    /// <summary>
    /// Retrieves vehicles that have no active rental.
    /// </summary>
    /// <param name="_">Unused query payload.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>Available vehicles.</returns>
    public async Task<IReadOnlyList<VehicleQueryResultDto>?> Handle(ListVehiclesQueryDto _, CancellationToken token)
        => await _repo.GetAvailableAsync(token);
}
