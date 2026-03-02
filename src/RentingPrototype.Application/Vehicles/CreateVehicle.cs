namespace RentingPrototype.Application.Vehicles.CreateVehicle;

public sealed record CreateVehicleCommand(
    string LicensePlate,
    string Make,
    string Model,
    DateTime ManufacturingDateUtc);