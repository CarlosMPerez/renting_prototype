using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicles.Ports;

namespace RentingPrototype.Application.Vehicles.Queries;

public sealed record ListVehiclesQueryDto;

public class GetAllVehiclesHandler : IQueryHandler<ListVehiclesQueryDto, IReadOnlyList<VehicleQueryResultDto>>
{
    private readonly IVehicleQueryRepository _repo;

    public GetAllVehiclesHandler(IVehicleQueryRepository repo)
        => _repo = repo;

    public Task<IReadOnlyList<VehicleQueryResultDto>> Handle(ListVehiclesQueryDto _, CancellationToken token)
        => _repo.GetAllAsync(token);
}
