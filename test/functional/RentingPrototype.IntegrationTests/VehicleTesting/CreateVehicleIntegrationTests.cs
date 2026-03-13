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
            brand = "Honda",
            model = "Civic",
            manufactureDateUtc = "2024-01-01T00:00:00Z"
        };

        var res = await client.PostAsJsonAsync("/vehicles", payload);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task PostVehicles_WritesDomainEventToLogsFile()
    {
        var contentRoot = Path.Combine(Path.GetTempPath(), $"rentingprototype-api-{Guid.NewGuid():N}");
        Directory.CreateDirectory(contentRoot);

        try
        {
            await using var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(b =>
                {
                    b.UseEnvironment("Testing");
                    b.UseContentRoot(contentRoot);
                });

            var client = factory.CreateClient();

            var payload = new
            {
                licensePlate = "1111-EVT",
                brand = "Seat",
                model = "Ibiza",
                manufactureDateUtc = "2024-01-01T00:00:00Z"
            };

            var response = await client.PostAsJsonAsync("/vehicles", payload);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var logFilePath = Path.Combine(contentRoot, "logs", "log.txt");
            Assert.True(File.Exists(logFilePath));

            var logText = await File.ReadAllTextAsync(logFilePath);
            Assert.Contains("VehicleCreatedDomainEvent", logText);
            Assert.Contains("1111-EVT", logText);
        }
        finally
        {
            if (Directory.Exists(contentRoot))
            {
                Directory.Delete(contentRoot, recursive: true);
            }
        }
    }
}
