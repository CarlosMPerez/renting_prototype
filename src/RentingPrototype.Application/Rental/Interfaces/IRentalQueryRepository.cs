using RentingPrototype.Application.Rental.Queries;

namespace RentingPrototype.Application.Rental.Interfaces;

public interface IRentalQueryRepository
{
    Task<RentalQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token);
}
