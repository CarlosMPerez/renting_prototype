using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Ports;

namespace RentingPrototype.Application.Vehicle.Queries;

public sealed record ListVehiclesQueryDto;

public class GetAllVehiclesQueryHandler : IQueryHandler<ListVehiclesQueryDto, IReadOnlyList<VehicleQueryResultDto>>
{
    private readonly IVehicleQueryRepository _repo;

    /// <summary>
    /// Creates a handler instance for listing all vehicles.
    /// </summary>
    /// <param name="repo">Vehicle query repository.</param>
    public GetAllVehiclesQueryHandler(IVehicleQueryRepository repo)
        => _repo = repo;

    /// <summary>
    /// Retrieves all vehicles.
    /// </summary>
    /// <param name="_">Unused query payload.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>All vehicles available in persistence.</returns>
    public async Task<IReadOnlyList<VehicleQueryResultDto>?> Handle(ListVehiclesQueryDto _, CancellationToken token)
        => await _repo.GetAllAsync(token);
}
