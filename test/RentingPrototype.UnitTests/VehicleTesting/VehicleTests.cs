using RentingPrototype.Domain.Vehicles;

namespace RentingPrototype.UnitTests.VehicleTesting;

public class VehicleTests
{
    [Fact]
    public void Create_RejectsVehicleOlderThan5Years()
    {
        var manufacturingDate = DateTime.UtcNow.AddYears(-5).AddDays(-1);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-TEST",
                make: "Toyota",
                model: "Corolla",
                manufacturingDateUtc: manufacturingDate,
                nowUtc: DateTime.UtcNow
                )
        );

        Assert.Contains("older than 5 years", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_AllowsVehicleExactly5YearsOld()
    {
        var manufacturingDate = DateTime.UtcNow.AddYears(-5); // borderline OK

        var v = Vehicle.Create(
            id: Guid.NewGuid(),
            licensePlate: "1234-ABC",
            make: "Toyota",
            model: "Corolla",
            manufacturingDateUtc: manufacturingDate,
            nowUtc: DateTime.UtcNow);

        Assert.Equal("1234-ABC", v.LicensePlate);
        Assert.Equal("Toyota", v.Make);
        Assert.Equal("Corolla", v.Model);
        Assert.Equal(manufacturingDate, v.ManufacturingDateUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsEmptyLicensePlate(string plate)
    {
        Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: plate,
                make: "Toyota",
                model: "Corolla",
                manufacturingDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow));
    }
    // if (string.IsNullOrWhiteSpace(make)) throw new ArgumentException("Make is required.", nameof(make));
    // if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model is required.", nameof(model));

    [Fact]
    public void Create_TrimsInputStrings()
    {
        var v = Vehicle.Create(
            id: Guid.NewGuid(),
            licensePlate: "  1234-ABC  ",
            make: "  Toyota ",
            model: " Corolla  ",
            manufacturingDateUtc: DateTime.UtcNow.AddYears(-3),
            nowUtc: DateTime.UtcNow);

        Assert.Equal("1234-ABC", v.LicensePlate);
        Assert.Equal("Toyota", v.Make);
        Assert.Equal("Corolla", v.Model);
    }

    [Fact]
    public void Create_RejectsLongLicensePlate()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                make: "Toyota",
                model: "Corolla",
                manufacturingDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("plate max length is 20", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsLongMake()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                make: new string('m', 101),
                model: "Corolla",
                manufacturingDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("make max length", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsLongModel()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                make: "Toyota",
                model: new string('m', 101),
                manufacturingDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("model max length", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_RejectsNullOrEmptyMake(string? make)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                make: make,
                model: "Corolla",
                manufacturingDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("make is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_RejectsNullOrEmptyModel(string? model)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                make: "Toyota",
                model: model,
                manufacturingDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("model is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
