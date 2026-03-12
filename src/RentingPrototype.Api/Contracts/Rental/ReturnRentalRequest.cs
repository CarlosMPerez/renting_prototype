using RentingPrototype.Api.Validation;

namespace RentingPrototype.Api.Contracts.Rental;

public sealed record ReturnRentalRequest(
    [property: NotEmptyGuid] Guid Id,
    DateTime EndDate);
