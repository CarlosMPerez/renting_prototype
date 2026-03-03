namespace RentingPrototype.Api.Contracts;

public sealed record CreateVehicleRequest(
    string LicensePlate,
    string Brand,
    string Model,
    DateTime ManufactureDateUtc
);
