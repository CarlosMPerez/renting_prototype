using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Ports;
using RentingPrototype.Application.RentalHistory.Ports;
using RentingPrototype.Application.Vehicle.Ports;
using RentingPrototype.Infrastructure.Persistence.Sqlite;
using RentingPrototype.Infrastructure.Persistence.SQLite;

namespace RentingPrototype.HostTests.HostTesting;

public sealed class HostConfigurationTests
{
    [Fact]
    public void Host_RegistersRequiredServices()
    {
        using var factory = new HostWebApplicationFactory();
        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        Assert.NotNull(services.GetService<ISqliteConnectionFactory>());
        Assert.IsType<SqliteUnitOfWork>(services.GetRequiredService<IUnitOfWork>());
        Assert.NotNull(services.GetService<IDomainEventDispatcher>());
        Assert.NotNull(services.GetService<IVehicleCommandRepository>());
        Assert.NotNull(services.GetService<IVehicleQueryRepository>());
        Assert.NotNull(services.GetService<IRentalCommandRepository>());
        Assert.NotNull(services.GetService<IRentalQueryRepository>());
        Assert.NotNull(services.GetService<IRentalHistoryQueryRepository>());
    }

    [Fact]
    public async Task Host_MapsBusinessEndpoints()
    {
        await using var factory = new HostWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/vehicles");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<VehicleDto>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload!);
    }

    [Fact]
    public async Task Host_ExposesOpenApiDocument()
    {
        await using var factory = new HostWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var openApiJson = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"openapi\"", openApiJson);
        Assert.Contains("\"/vehicles\"", openApiJson);
    }

    [Fact]
    public async Task Host_ExposesScalarUi()
    {
        await using var factory = new HostWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/scalar/v1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("scalar", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Host_UsesIndependentInMemoryDatabasePerFactory()
    {
        var uniquePlate = $"{Guid.NewGuid():N}"[..8].ToUpperInvariant();

        await using var firstFactory = new HostWebApplicationFactory();
        var firstClient = firstFactory.CreateClient();

        var createResponse = await firstClient.PostAsJsonAsync("/vehicles", new
        {
            licensePlate = $"HT-{uniquePlate}",
            brand = "Host",
            model = "Isolation",
            manufactureDateUtc = DateTime.UtcNow.AddYears(-1)
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var firstVehicles = await firstClient.GetFromJsonAsync<List<VehicleDto>>("/vehicles");
        Assert.NotNull(firstVehicles);
        Assert.Contains(firstVehicles!, v => v.LicensePlate == $"HT-{uniquePlate}");

        await using var secondFactory = new HostWebApplicationFactory();
        var secondClient = secondFactory.CreateClient();
        var secondVehicles = await secondClient.GetFromJsonAsync<List<VehicleDto>>("/vehicles");

        Assert.NotNull(secondVehicles);
        Assert.DoesNotContain(secondVehicles!, v => v.LicensePlate == $"HT-{uniquePlate}");
    }

    private sealed record VehicleDto(
        Guid Id,
        string LicensePlate,
        string Brand,
        string Model,
        DateTime ManufactureDate);
}
