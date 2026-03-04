using RentingPrototype.Domain.VehicleDomain;

namespace RentingPrototype.UnitTests.VehicleTesting;

public class VehicleTests
{
    [Fact]
    public void Create_RejectsVehicleOlderThan5Years()
    {
        var manufactureDate = DateTime.UtcNow.AddYears(-5).AddDays(-1);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-TEST",
                brand: "Toyota",
                model: "Corolla",
                manufactureDateUtc: manufactureDate,
                nowUtc: DateTime.UtcNow
                )
        );

        Assert.Contains("older than 5 years", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_AllowsVehicleExactly5YearsOld()
    {
        var manufactureDate = DateTime.UtcNow.AddYears(-5); // borderline OK

        var v = Vehicle.Create(
            id: Guid.NewGuid(),
            licensePlate: "1234-ABC",
            brand: "Toyota",
            model: "Corolla",
            manufactureDateUtc: manufactureDate,
            nowUtc: DateTime.UtcNow);

        Assert.Equal("1234-ABC", v.LicensePlate);
        Assert.Equal("Toyota", v.Brand);
        Assert.Equal("Corolla", v.Model);
        Assert.Equal(manufactureDate, v.ManufactureDateUtc);
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
                brand: "Toyota",
                model: "Corolla",
                manufactureDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow));
    }

    [Fact]
    public void Create_TrimsInputStrings()
    {
        var v = Vehicle.Create(
            id: Guid.NewGuid(),
            licensePlate: "  1234-ABC  ",
            brand: "  Toyota ",
            model: " Corolla  ",
            manufactureDateUtc: DateTime.UtcNow.AddYears(-3),
            nowUtc: DateTime.UtcNow);

        Assert.Equal("1234-ABC", v.LicensePlate);
        Assert.Equal("Toyota", v.Brand);
        Assert.Equal("Corolla", v.Model);
    }

    [Fact]
    public void Create_RejectsLongLicensePlate()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                brand: "Toyota",
                model: "Corolla",
                manufactureDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("plate max length is 20", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsLongBrand()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                brand: new string('m', 101),
                model: "Corolla",
                manufactureDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("brand max length", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsLongModel()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                brand: "Toyota",
                model: new string('m', 101),
                manufactureDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );

        Assert.Contains("model max length", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_RejectsNullOrEmptyBrand(string? brand)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                brand: brand,
                model: "Corolla",
                manufactureDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );
#pragma warning restore CS8604 // Possible null reference argument.

        Assert.Contains("brand is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_RejectsNullOrEmptyModel(string? model)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var ex = Assert.Throws<ArgumentException>(() =>
            Vehicle.Create(
                id: Guid.NewGuid(),
                licensePlate: "1234-ABC",
                brand: "Toyota",
                model: model,
                manufactureDateUtc: DateTime.UtcNow.AddYears(-3),
                nowUtc: DateTime.UtcNow)
        );
#pragma warning restore CS8604 // Possible null reference argument.

        Assert.Contains("model is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
