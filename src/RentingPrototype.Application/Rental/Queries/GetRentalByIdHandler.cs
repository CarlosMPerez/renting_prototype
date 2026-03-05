using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Interfaces;

namespace RentingPrototype.Application.Rental.Queries;

public sealed record RentalQueryFilterDto(Guid Id);

public sealed class GetRentalByIdQueryHandler : IQueryHandler<RentalQueryFilterDto, RentalQueryResultDto>
{
    private readonly IRentalQueryRepository _repo;

    /// <summary>
    /// Creates a handler instance for querying rentals by identifier.
    /// </summary>
    /// <param name="repo">Rental query repository.</param>
    public GetRentalByIdQueryHandler(IRentalQueryRepository repo) => _repo = repo;

    /// <summary>
    /// Retrieves a rental by identifier.
    /// </summary>
    /// <param name="query">Rental query filter.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The rental when found; otherwise <c>null</c>.</returns>
    public Task<RentalQueryResultDto?> Handle(RentalQueryFilterDto query, CancellationToken token)
        => _repo.GetByIdAsync(query.Id, token);
}
