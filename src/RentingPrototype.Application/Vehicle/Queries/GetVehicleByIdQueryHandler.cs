using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Interfaces;

namespace RentingPrototype.Application.Vehicle.Queries;

public sealed record VehicleQueryFilterDto(Guid Id);

public sealed class GetVehicleByIdQueryHandler : IQueryHandler<VehicleQueryFilterDto, VehicleQueryResultDto>
{
    private readonly IVehicleQueryRepository _repo;
    public GetVehicleByIdQueryHandler(IVehicleQueryRepository repo) => _repo = repo;
    public Task<VehicleQueryResultDto?> Handle(VehicleQueryFilterDto query, CancellationToken token)
        => _repo.GetByIdAsync(query.Id, token);
}
