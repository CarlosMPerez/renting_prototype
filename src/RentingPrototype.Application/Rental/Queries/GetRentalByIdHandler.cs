using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Interfaces;

namespace RentingPrototype.Application.Rental.Queries;

public sealed record RentalQueryFilterDto(Guid Id);

public sealed class GetRentalByIdQueryHandler : IQueryHandler<RentalQueryFilterDto, RentalQueryResultDto>
{
    private readonly IRentalQueryRepository _repo;
    public GetRentalByIdQueryHandler(IRentalQueryRepository repo) => _repo = repo;
    public Task<RentalQueryResultDto?> Handle(RentalQueryFilterDto query, CancellationToken token)
        => _repo.GetByIdAsync(query.Id, token);
}
