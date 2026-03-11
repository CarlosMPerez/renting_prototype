using RentingPrototype.Domain.Common;

namespace RentingPrototype.Domain.RentalDomain.Events;

public sealed record VehicleRentedDomainEvent(
    Guid RentalId,
    Guid VehicleId,
    Guid CustomerId,
    DateTime StartDate,
    DateTime OccurredOnUtc) : IDomainEvent;
