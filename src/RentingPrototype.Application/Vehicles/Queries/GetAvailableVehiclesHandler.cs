using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicles.Ports;

namespace RentingPrototype.Application.Vehicles.Queries;

public sealed record ListAvailableVehiclesQueryDto;

public class GetAvailableVehiclesHandler : IQueryHandler<ListVehiclesQueryDto, IReadOnlyList<VehicleQueryResultDto>>
{
    private readonly IVehicleQueryRepository _repo;

    public GetAvailableVehiclesHandler(IVehicleQueryRepository repo)
        => _repo = repo;

    public Task<IReadOnlyList<VehicleQueryResultDto>> Handle(ListVehiclesQueryDto _, CancellationToken token)
        => _repo.GetAvailableAsync(token);
}