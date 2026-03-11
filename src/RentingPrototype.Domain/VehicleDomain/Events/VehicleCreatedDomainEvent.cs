using RentingPrototype.Domain.Common;
using RentingPrototype.Domain.VehicleDomain.ValueObjects;

namespace RentingPrototype.Domain.VehicleDomain.Events;

public sealed record VehicleCreatedDomainEvent(
    VehicleId VehicleId,
    string LicensePlate,
    DateTime OccurredOnUtc) : IDomainEvent;
