using RentingPrototype.Domain.Common;
using RentingPrototype.Domain.Common.Exceptions;
using RentingPrototype.Domain.VehicleDomain.Events;
using RentingPrototype.Domain.VehicleDomain.ValueObjects;

namespace RentingPrototype.Domain.VehicleDomain;

public sealed class Vehicle : AggregateRoot
{
    public VehicleId Id { get; private set; }
    public LicensePlate LicensePlate { get; private set; }
    public string Brand { get; }
    public string Model { get; }
    public ManufactureDateOnly ManufactureDateUtc { get; }

    /// <summary>
    /// Initializes a vehicle aggregate instance.
    /// </summary>
    private Vehicle(VehicleId id, LicensePlate licensePlate,
                    string brand, string model,
                    ManufactureDateOnly manufactureDateUtc)
    {
        Id = id;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        ManufactureDateUtc = manufactureDateUtc;
    }

    /// <summary>
    /// Creates a vehicle aggregate enforcing the business constraints.
    /// </summary>
    /// <param name="id">Vehicle identifier.</param>
    /// <param name="licensePlate">Vehicle license plate.</param>
    /// <param name="brand">Vehicle brand.</param>
    /// <param name="model">Vehicle model.</param>
    /// <param name="manufactureDateUtc">Vehicle manufacture date in UTC.</param>
    /// <param name="nowUtc">Current UTC date used to validate age rules.</param>
    /// <returns>A valid <see cref="Vehicle"/> instance.</returns>
    public static Vehicle Create(Guid id, string licensePlate,
                                    string brand, string model,
                                    DateTime manufactureDateUtc,
                                    DateTime nowUtc)
    {
        if (id == Guid.Empty) throw new DomainValidationException("Id cannot be empty.");
        if (string.IsNullOrWhiteSpace(brand)) throw new DomainValidationException("Brand is required.");
        if (brand.Length > 100) throw new DomainValidationException("Brand max length is 100.");
        if (string.IsNullOrWhiteSpace(model)) throw new DomainValidationException("Model is required.");
        if (model.Length > 100) throw new DomainValidationException("Model max length is 100.");

        var vehicle = new Vehicle(
            VehicleId.From(id),
            LicensePlate.Create(licensePlate),
            brand.Trim(),
            model.Trim(),
            ManufactureDateOnly.Create(manufactureDateUtc, nowUtc));

        vehicle.AddDomainEvent(new VehicleCreatedDomainEvent(
            vehicle.Id,
            vehicle.LicensePlate.Value,
            nowUtc));

        return vehicle;
    }
}
