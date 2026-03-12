using RentingPrototype.Api.Validation;

namespace RentingPrototype.Api.Contracts.Rental;

public sealed record CreateRentalRequest(
    [property: NotEmptyGuid] Guid CustomerId,
    [property: NotEmptyGuid] Guid VehicleId,
    DateTime StartDate);
