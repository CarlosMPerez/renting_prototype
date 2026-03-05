using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;

namespace RentingPrototype.UnitTests.RentalHistoryTesting;

public sealed class RentalHistoryResultDtoTests
{
    [Fact]
    public void VehicleResultDto_StringConstructor_ParsesGuidAndDates()
    {
        var customerId = Guid.NewGuid();

        var dto = new VehicleRentalHistoryResultDto(
            customerId.ToString("D"),
            "DOC009",
            "Miguel",
            "Ruiz",
            "2025-03-18",
            "2025-04-12");

        Assert.Equal(customerId, dto.CustomerId);
        Assert.Equal("DOC009", dto.DocumentId);
        Assert.Equal("Miguel", dto.Name);
        Assert.Equal("Ruiz", dto.Surname);
        Assert.Equal(new DateOnly(2025, 3, 18), dto.StartDate);
        Assert.Equal(new DateOnly(2025, 4, 12), dto.EndDate);
    }

    [Fact]
    public void VehicleResultDto_StringConstructor_AllowsNullEndDate()
    {
        var dto = new VehicleRentalHistoryResultDto(
            Guid.NewGuid().ToString("D"),
            "DOC001",
            "Carlos",
            "Perez",
            "2026-03-01",
            null);

        Assert.Equal(new DateOnly(2026, 3, 1), dto.StartDate);
        Assert.Null(dto.EndDate);
    }

    [Fact]
    public void CustomerResultDto_StringConstructor_ParsesGuidAndDates()
    {
        var vehicleId = Guid.NewGuid();

        var dto = new CustomerRentalHistoryResultDto(
            vehicleId.ToString("D"),
            "0010-AAA",
            "Hyundai",
            "i20",
            "2024-05-10",
            "2024-05-30");

        Assert.Equal(vehicleId, dto.VehicleId);
        Assert.Equal("0010-AAA", dto.LicensePlate);
        Assert.Equal("Hyundai", dto.Brand);
        Assert.Equal("i20", dto.Model);
        Assert.Equal(new DateOnly(2024, 5, 10), dto.StartDate);
        Assert.Equal(new DateOnly(2024, 5, 30), dto.EndDate);
    }

    [Fact]
    public void CustomerResultDto_StringConstructor_AllowsNullEndDate()
    {
        var dto = new CustomerRentalHistoryResultDto(
            Guid.NewGuid().ToString("D"),
            "0001-AAA",
            "Toyota",
            "Corolla",
            "2026-03-01",
            null);

        Assert.Equal(new DateOnly(2026, 3, 1), dto.StartDate);
        Assert.Null(dto.EndDate);
    }
}
