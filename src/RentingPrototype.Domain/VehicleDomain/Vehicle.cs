using RentingPrototype.Domain.VehicleDomain.ValueObjects;

namespace RentingPrototype.Domain.VehicleDomain;

public sealed class Vehicle
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
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand is required.", nameof(brand));
        if (brand.Length > 100) throw new ArgumentException("Brand max length is 100.", nameof(brand));
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model is required.", nameof(model));
        if (model.Length > 100) throw new ArgumentException("Model max length is 100.", nameof(model));

        return new Vehicle(
            VehicleId.From(id),
            LicensePlate.Create(licensePlate),
            brand.Trim(),
            model.Trim(),
            ManufactureDateOnly.Create(manufactureDateUtc, nowUtc));
    }
}
