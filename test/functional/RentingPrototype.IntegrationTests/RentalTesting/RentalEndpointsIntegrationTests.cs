using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RentingPrototype.IntegrationTests.RentalTesting;

public sealed class RentalEndpointsIntegrationTests
{
    private const string Customer1 = "a1111111-0000-0000-0000-000000000001";
    private const string Customer2 = "a1111111-0000-0000-0000-000000000002";
    private const string Vehicle1 = "b2222222-0000-0000-0000-000000000001";
    private const string Vehicle2 = "b2222222-0000-0000-0000-000000000002";

    [Fact]
    public async Task RentVehicle_Returns201_ForValidRequest()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var payload = new
        {
            customerId = Customer1,
            vehicleId = Vehicle1,
            startDate = DateTime.UtcNow.AddDays(-1)
        };

        var response = await client.PostAsJsonAsync("/rentals/rent-vehicle", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task RentVehicle_WhenCustomerAlreadyHasOpenRental_Returns409()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var startDate = DateTime.UtcNow.AddDays(-1);

        var first = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
        {
            customerId = Customer1,
            vehicleId = Vehicle1,
            startDate
        });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
        {
            customerId = Customer1,
            vehicleId = Vehicle2,
            startDate
        });

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task RentVehicle_WhenVehicleAlreadyHasOpenRental_Returns409()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var startDate = DateTime.UtcNow.AddDays(-1);

        var first = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
        {
            customerId = Customer1,
            vehicleId = Vehicle1,
            startDate
        });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
        {
            customerId = Customer2,
            vehicleId = Vehicle1,
            startDate
        });

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task ReturnVehicle_Returns200_ForExistingOpenRental()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var rentResponse = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
        {
            customerId = Customer1,
            vehicleId = Vehicle1,
            startDate = DateTime.UtcNow.AddDays(-2)
        });
        Assert.Equal(HttpStatusCode.Created, rentResponse.StatusCode);

        var created = await rentResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        Assert.NotNull(created);

        var returnResponse = await client.PostAsJsonAsync("/rentals/return-vehicle", new
        {
            id = created!.Id,
            endDate = DateTime.UtcNow.AddDays(-1)
        });

        Assert.Equal(HttpStatusCode.OK, returnResponse.StatusCode);
    }

    [Fact]
    public async Task ReturnVehicle_WhenEndDateBeforeStartDate_Returns400()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var startDate = DateTime.UtcNow.AddDays(-1);
        var rentResponse = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
        {
            customerId = Customer1,
            vehicleId = Vehicle1,
            startDate
        });
        Assert.Equal(HttpStatusCode.Created, rentResponse.StatusCode);

        var created = await rentResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        Assert.NotNull(created);

        var returnResponse = await client.PostAsJsonAsync("/rentals/return-vehicle", new
        {
            id = created!.Id,
            endDate = startDate.AddDays(-1)
        });

        Assert.Equal(HttpStatusCode.BadRequest, returnResponse.StatusCode);
    }

    [Fact]
    public async Task RentAndReturnVehicle_WritesRentedAndReturnedEventsToLogsFile()
    {
        var contentRoot = Path.Combine(Path.GetTempPath(), $"rentingprototype-api-{Guid.NewGuid():N}");
        Directory.CreateDirectory(contentRoot);

        try
        {
            await using var factory = CreateFactory(contentRoot);
            var client = factory.CreateClient();

            var rentResponse = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
            {
                customerId = Customer1,
                vehicleId = Vehicle1,
                startDate = DateTime.UtcNow.AddDays(-2)
            });
            Assert.Equal(HttpStatusCode.Created, rentResponse.StatusCode);

            var created = await rentResponse.Content.ReadFromJsonAsync<CreatedResponse>();
            Assert.NotNull(created);

            var returnResponse = await client.PostAsJsonAsync("/rentals/return-vehicle", new
            {
                id = created!.Id,
                endDate = DateTime.UtcNow.AddDays(-1)
            });
            Assert.Equal(HttpStatusCode.OK, returnResponse.StatusCode);

            var logFilePath = Path.Combine(contentRoot, "logs", "log.txt");
            Assert.True(File.Exists(logFilePath));

            var logText = await File.ReadAllTextAsync(logFilePath);
            Assert.Contains("VehicleRentedDomainEvent", logText);
            Assert.Contains("VehicleReturnedDomainEvent", logText);
        }
        finally
        {
            if (Directory.Exists(contentRoot))
            {
                Directory.Delete(contentRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task RentVehicle_WhenBusinessRuleFails_WritesExceptionToLogsFile()
    {
        var contentRoot = Path.Combine(Path.GetTempPath(), $"rentingprototype-api-{Guid.NewGuid():N}");
        Directory.CreateDirectory(contentRoot);

        try
        {
            await using var factory = CreateFactory(contentRoot);
            var client = factory.CreateClient();

            var startDate = DateTime.UtcNow.AddDays(-1);

            var first = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
            {
                customerId = Customer1,
                vehicleId = Vehicle1,
                startDate
            });
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

            var second = await client.PostAsJsonAsync("/rentals/rent-vehicle", new
            {
                customerId = Customer1,
                vehicleId = Vehicle2,
                startDate
            });
            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

            var logFilePath = Path.Combine(contentRoot, "logs", "log.txt");
            Assert.True(File.Exists(logFilePath));

            var logText = await File.ReadAllTextAsync(logFilePath);
            Assert.Contains("BusinessRuleViolationException", logText);
            Assert.Contains("\"Type\":\"exception\"", logText);
        }
        finally
        {
            if (Directory.Exists(contentRoot))
            {
                Directory.Delete(contentRoot, recursive: true);
            }
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(string? contentRoot = null)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b =>
            {
                b.UseEnvironment("Testing");
                if (!string.IsNullOrWhiteSpace(contentRoot))
                {
                    b.UseContentRoot(contentRoot);
                }
            });
    }

    private sealed record CreatedResponse(Guid Id);
}
