using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Interfaces;

namespace RentingPrototype.Application.Vehicle.Queries;

public sealed record ListVehiclesQueryDto;

public class GetAllVehiclesQueryHandler : IQueryHandler<ListVehiclesQueryDto, IReadOnlyList<VehicleQueryResultDto>>
{
    private readonly IVehicleQueryRepository _repo;

    public GetAllVehiclesQueryHandler(IVehicleQueryRepository repo)
        => _repo = repo;

    public Task<IReadOnlyList<VehicleQueryResultDto>> Handle(ListVehiclesQueryDto _, CancellationToken token)
        => _repo.GetAllAsync(token);
}
