using System.Net;
using System.Net.Http.Json;
using RentingPrototype.TestUtilities;

namespace RentingPrototype.IntegrationTests.RentalHistoryTesting;

public sealed class RentalHistoryEndpointsIntegrationTests
{
    private const string VehicleWithHistory = "b2222222-0000-0000-0000-000000000001";
    private const string CustomerWithHistory = "a1111111-0000-0000-0000-000000000001";

    [Fact]
    public async Task GetVehicleRentalHistory_Returns200_WithCustomerData()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/rentalhistory/vehicles/{VehicleWithHistory}/rental-history");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<VehicleRentalHistoryItemDto>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload!);

        Assert.All(payload!, item =>
        {
            Assert.NotEqual(Guid.Empty, item.CustomerId);
            Assert.False(string.IsNullOrWhiteSpace(item.DocumentId));
            Assert.False(string.IsNullOrWhiteSpace(item.Name));
            Assert.False(string.IsNullOrWhiteSpace(item.Surname));
        });
    }

    [Fact]
    public async Task GetVehicleRentalHistory_IsOrderedByStartDateDesc()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var payload = await client.GetFromJsonAsync<List<VehicleRentalHistoryItemDto>>(
            $"/rentalhistory/vehicles/{VehicleWithHistory}/rental-history");

        Assert.NotNull(payload);
        Assert.NotEmpty(payload!);

        var starts = payload!.Select(x => x.StartDate).ToList();
        var expected = starts.OrderByDescending(x => x).ToList();

        Assert.Equal(expected, starts);
    }

    [Fact]
    public async Task GetCustomerRentalHistory_Returns200_WithVehicleData()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/rentalhistory/customers/{CustomerWithHistory}/rental-history");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<CustomerRentalHistoryItemDto>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload!);

        Assert.All(payload!, item =>
        {
            Assert.NotEqual(Guid.Empty, item.VehicleId);
            Assert.False(string.IsNullOrWhiteSpace(item.LicensePlate));
            Assert.False(string.IsNullOrWhiteSpace(item.Brand));
            Assert.False(string.IsNullOrWhiteSpace(item.Model));
        });
    }

    [Fact]
    public async Task GetCustomerRentalHistory_IsOrderedByStartDateDesc()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var payload = await client.GetFromJsonAsync<List<CustomerRentalHistoryItemDto>>(
            $"/rentalhistory/customers/{CustomerWithHistory}/rental-history");

        Assert.NotNull(payload);
        Assert.NotEmpty(payload!);

        var starts = payload!.Select(x => x.StartDate).ToList();
        var expected = starts.OrderByDescending(x => x).ToList();

        Assert.Equal(expected, starts);
    }

    [Fact]
    public async Task GetVehicleRentalHistory_ForUnknownVehicle_ReturnsEmptyCollection()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var payload = await client.GetFromJsonAsync<List<VehicleRentalHistoryItemDto>>(
            $"/rentalhistory/vehicles/{Guid.NewGuid():D}/rental-history");

        Assert.NotNull(payload);
        Assert.Empty(payload!);
    }

    [Fact]
    public async Task GetCustomerRentalHistory_ForUnknownCustomer_ReturnsEmptyCollection()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var payload = await client.GetFromJsonAsync<List<CustomerRentalHistoryItemDto>>(
            $"/rentalhistory/customers/{Guid.NewGuid():D}/rental-history");

        Assert.NotNull(payload);
        Assert.Empty(payload!);
    }

    private static TestingWebApplicationFactory CreateFactory()
    {
        return new TestingWebApplicationFactory();
    }

    private sealed record VehicleRentalHistoryItemDto(
        Guid CustomerId,
        string DocumentId,
        string Name,
        string Surname,
        DateOnly StartDate,
        DateOnly? EndDate);

    private sealed record CustomerRentalHistoryItemDto(
        Guid VehicleId,
        string LicensePlate,
        string Brand,
        string Model,
        DateOnly StartDate,
        DateOnly? EndDate);
}
