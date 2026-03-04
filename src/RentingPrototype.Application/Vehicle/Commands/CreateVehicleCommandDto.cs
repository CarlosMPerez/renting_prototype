namespace RentingPrototype.Application.Vehicle.Commands;

public sealed record CreateVehicleCommandDto(
    string LicensePlate,
    string Brand,
    string Model,
    DateTime ManufactureDateUtc);