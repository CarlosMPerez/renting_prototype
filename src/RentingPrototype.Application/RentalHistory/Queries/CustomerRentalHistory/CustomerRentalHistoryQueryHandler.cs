using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.RentalHistory.Ports;

namespace RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;

public sealed record CustomerRentalHistoryFilterDto(Guid customerId);
public sealed class CustomerRentalHistoryQueryHandler : IQueryHandler<CustomerRentalHistoryFilterDto, IReadOnlyList<CustomerRentalHistoryResultDto>>
{
    private readonly IRentalHistoryQueryRepository _repo;

    /// <summary>
    /// Creates a handler instance for customer rental history queries.
    /// </summary>
    /// <param name="repo">Rental history query repository.</param>
    public CustomerRentalHistoryQueryHandler(IRentalHistoryQueryRepository repo) => _repo = repo;

    /// <summary>
    /// Retrieves rental history entries for a customer.
    /// </summary>
    /// <param name="query">Customer rental history filter.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>History rows containing vehicle and rental period data.</returns>
    public Task<IReadOnlyList<CustomerRentalHistoryResultDto>?> Handle(CustomerRentalHistoryFilterDto query, CancellationToken token)
        => _repo.GetByCustomerIdAsync(query.customerId, token);
}
