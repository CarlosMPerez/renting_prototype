using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RentingPrototype.IntegrationTests.VehicleTesting;

public sealed class CreateVehicleIntegrationTests
{
    [Fact]
    public async Task PostVehicles_Returns201()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseEnvironment("Testing"));

        var client = factory.CreateClient();

        var payload = new
        {
            licensePlate = "9999-ZZZ",
            make = "Honda",
            model = "Civic",
            manufacturingDateUtc = "2024-01-01T00:00:00Z"
        };

        var res = await client.PostAsJsonAsync("/vehicles", payload);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }
}