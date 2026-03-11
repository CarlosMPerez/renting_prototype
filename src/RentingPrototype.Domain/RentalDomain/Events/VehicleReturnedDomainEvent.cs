using RentingPrototype.Domain.Common;

namespace RentingPrototype.Domain.RentalDomain.Events;

public sealed record VehicleReturnedDomainEvent(
    Guid RentalId,
    Guid VehicleId,
    Guid CustomerId,
    DateTime EndDate,
    DateTime OccurredOnUtc) : IDomainEvent;
