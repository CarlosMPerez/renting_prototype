using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Interfaces;

namespace RentingPrototype.Application.Vehicle.Queries;

public sealed record ListAvailableVehiclesQueryDto;

public class GetAvailableVehiclesQueryHandler : IQueryHandler<ListVehiclesQueryDto, IReadOnlyList<VehicleQueryResultDto>>
{
    private readonly IVehicleQueryRepository _repo;

    public GetAvailableVehiclesQueryHandler(IVehicleQueryRepository repo)
        => _repo = repo;

    public Task<IReadOnlyList<VehicleQueryResultDto>> Handle(ListVehiclesQueryDto _, CancellationToken token)
        => _repo.GetAvailableAsync(token);
}