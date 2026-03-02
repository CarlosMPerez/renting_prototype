namespace RentingPrototype.Api.Contracts;

public sealed record CreateVehicleRequest(
    string LicensePlate,
    string Make,
    string Model,
    DateTime ManufacturingDateUtc
);
