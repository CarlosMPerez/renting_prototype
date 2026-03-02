namespace RentingPrototype.Domain.Vehicles;

public sealed class Vehicle
{
    public Guid Id { get; }
    public string LicensePlate { get; }
    public string Make { get; }
    public string Model { get; }
    public DateTime ManufacturingDateUtc { get; }

    private Vehicle(Guid id, string licensePlate,
                    string make, string model,
                    DateTime manufacturingDateUtc)
    {
        Id = id;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        ManufacturingDateUtc = manufacturingDateUtc;
    }

    public static Vehicle Create(Guid id, string licensePlate,
                                    string make, string model,
                                    DateTime manufacturingDateUtc,
                                    DateTime nowUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(licensePlate)) throw new ArgumentException("License plate is required.", nameof(licensePlate));
        if (licensePlate.Length > 20) throw new ArgumentException("License plate max length is 20.", nameof(licensePlate));
        if (string.IsNullOrWhiteSpace(make)) throw new ArgumentException("Make is required.", nameof(make));
        if (make.Length > 100) throw new ArgumentException("Make max length is 100.", nameof(make));
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model is required.", nameof(model));
        if (model.Length > 100) throw new ArgumentException("Model max length is 100.", nameof(model));

        var minDate = nowUtc.Date.AddYears(-5);
        if (manufacturingDateUtc.Date < minDate)
            throw new InvalidOperationException("Vehicle is older than 5 years and cannot be registered.");

        return new Vehicle(
            id,
            licensePlate.Trim(),
            make.Trim(),
            model.Trim(),
            DateTime.SpecifyKind(manufacturingDateUtc, DateTimeKind.Utc));
    }
}