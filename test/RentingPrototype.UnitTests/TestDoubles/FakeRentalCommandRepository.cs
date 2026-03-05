using RentingPrototype.Application.Rental.Interfaces;
using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.UnitTests.TestDoubles;

public sealed class FakeRentalCommandRepository : IRentalCommandRepository
{
    public bool Created { get; private set; }
    public bool Updated { get; private set; }

    public bool ThrowOnCreate { get; set; }
    public bool ThrowOnUpdate { get; set; }

    public RentalDomain.Rental? LastCreatedRental { get; private set; }
    public RentalDomain.Rental? LastUpdatedRental { get; private set; }

    public Task CreateAsync(RentalDomain.Rental rental, CancellationToken token)
    {
        if (ThrowOnCreate) throw new InvalidOperationException("DB blew up!!");

        Created = true;
        LastCreatedRental = rental;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RentalDomain.Rental rental, CancellationToken token)
    {
        if (ThrowOnUpdate) throw new InvalidOperationException("DB blew up!!");

        Updated = true;
        LastUpdatedRental = rental;
        return Task.CompletedTask;
    }
}
