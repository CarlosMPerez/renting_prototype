namespace RentingPrototype.Domain.VehicleDomain;

public sealed class Vehicle
{
    public Guid Id { get; }
    public string LicensePlate { get; }
    public string Brand { get; }
    public string Model { get; }
    public DateTime ManufactureDateUtc { get; }

    private Vehicle(Guid id, string licensePlate,
                    string brand, string model,
                    DateTime manufactureDateUtc)
    {
        Id = id;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        ManufactureDateUtc = manufactureDateUtc;
    }

    public static Vehicle Create(Guid id, string licensePlate,
                                    string brand, string model,
                                    DateTime manufactureDateUtc,
                                    DateTime nowUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(licensePlate)) throw new ArgumentException("License plate is required.", nameof(licensePlate));
        if (licensePlate.Length > 20) throw new ArgumentException("License plate max length is 20.", nameof(licensePlate));
        if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand is required.", nameof(brand));
        if (brand.Length > 100) throw new ArgumentException("Brand max length is 100.", nameof(brand));
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model is required.", nameof(model));
        if (model.Length > 100) throw new ArgumentException("Model max length is 100.", nameof(model));

        var minDate = nowUtc.Date.AddYears(-5);
        if (manufactureDateUtc.Date < minDate)
            throw new InvalidOperationException("Vehicle is older than 5 years and cannot be registered.");

        return new Vehicle(
            id,
            licensePlate.Trim(),
            brand.Trim(),
            model.Trim(),
            DateTime.SpecifyKind(manufactureDateUtc, DateTimeKind.Utc));
    }
}