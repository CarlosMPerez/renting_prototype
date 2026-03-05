using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RentingPrototype.IntegrationTests.VehicleTesting;

public sealed class VehicleReadAndValidationIntegrationTests
{
    [Fact]
    public async Task GetAvailableVehicles_DecreasesAfterRentingOneVehicle()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var before = await client.GetFromJsonAsync<List<VehicleItemDto>>("/vehicles/available");
        Assert.NotNull(before);

        var rentResponse = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
        {
            customerId = "a1111111-0000-0000-0000-000000000001",
            vehicleId = "b2222222-0000-0000-0000-000000000001",
            startDate = DateTime.UtcNow.AddDays(-1)
        });
        Assert.Equal(HttpStatusCode.Created, rentResponse.StatusCode);

        var after = await client.GetFromJsonAsync<List<VehicleItemDto>>("/vehicles/available");
        Assert.NotNull(after);

        Assert.Equal(before!.Count - 1, after!.Count);
    }

    [Fact]
    public async Task PostVehicles_WhenVehicleTooOld_Returns400()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var payload = new
        {
            licensePlate = "8888-YYY",
            brand = "Honda",
            model = "Accord",
            manufactureDateUtc = DateTime.UtcNow.AddYears(-6)
        };

        var response = await client.PostAsJsonAsync("/vehicles", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseEnvironment("Testing"));
    }

    private sealed record VehicleItemDto(Guid Id, string LicensePlate, string Brand, string Model, DateTime ManufactureDate);
}
