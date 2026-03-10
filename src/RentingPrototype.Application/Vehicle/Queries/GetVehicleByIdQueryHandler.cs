using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Ports;

namespace RentingPrototype.Application.Vehicle.Queries;

public sealed record VehicleQueryFilterDto(Guid Id);

public sealed class GetVehicleByIdQueryHandler : IQueryHandler<VehicleQueryFilterDto, VehicleQueryResultDto>
{
    private readonly IVehicleQueryRepository _repo;

    /// <summary>
    /// Creates a handler instance for querying vehicles by identifier.
    /// </summary>
    /// <param name="repo">Vehicle query repository.</param>
    public GetVehicleByIdQueryHandler(IVehicleQueryRepository repo) => _repo = repo;

    /// <summary>
    /// Retrieves a vehicle by identifier.
    /// </summary>
    /// <param name="query">Vehicle query filter.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The vehicle when found; otherwise <c>null</c>.</returns>
    public Task<VehicleQueryResultDto?> Handle(VehicleQueryFilterDto query, CancellationToken token)
        => _repo.GetByIdAsync(query.Id, token);
}
