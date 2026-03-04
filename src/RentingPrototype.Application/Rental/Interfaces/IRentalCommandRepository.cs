using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Application.Rental.Interfaces;

public interface IRentalCommandRepository
{
    Task CreateAsync(RentalDomain.Rental rental, CancellationToken token);
    Task UpdateAsync(RentalDomain.Rental rental, CancellationToken token);
}
