using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RentingPrototype.Api.Contracts.Vehicle;
using RentingPrototype.Api.Validation;
using RentingPrototype.Application.Vehicle.Commands;
using RentingPrototype.Application.Vehicle.Queries;

namespace RentingPrototype.Api.Endpoints.Vehicle;

public static class VehicleEndpoints
{
    /// <summary>
    /// Registers vehicle endpoints.
    /// </summary>
    /// <param name="app">Application route builder.</param>
    /// <returns>The configured route group.</returns>
    public static IEndpointRouteBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/vehicles").WithTags("Vehicles");

        // GET/vehicles/{id}
        group.MapGet("/{id:guid}", GetVehicleById).WithName("Get Vehicle By Id");

        // GET /vehicles
        group.MapGet("/", GetAllVehicles).WithName("Get All Vehicles");

        // GET /vehicles/available
        group.MapGet("/available", GetAvailableVehicles).WithName("Get Available Vehicles");

        // POST 
        group.MapPost("/", CreateVehicle);

        return group;
    }

    /// <summary>
    /// Gets a vehicle by identifier.
    /// </summary>
    /// <param name="id">Vehicle identifier.</param>
    /// <param name="handler">Query handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result containing vehicle data or not found.</returns>
    private static async Task<IResult> GetVehicleById(Guid id, GetVehicleByIdQueryHandler handler, CancellationToken token)
    {
        var vehicle = await handler.Handle(new VehicleQueryFilterDto(id), token);
        return vehicle is null ? Results.NotFound() : Results.Ok(vehicle);
    }

    /// <summary>
    /// Gets all registered vehicles.
    /// </summary>
    /// <param name="handler">Query handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result containing the vehicle collection.</returns>
    private static async Task<IResult> GetAllVehicles(GetAllVehiclesQueryHandler handler, CancellationToken token)
    {
        var vehicles = await handler.Handle(new ListVehiclesQueryDto(), token);
        return Results.Ok(vehicles);
    }

    /// <summary>
    /// Gets vehicles currently available for renting.
    /// </summary>
    /// <param name="handler">Query handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result containing the available vehicle collection.</returns>
    private static async Task<IResult> GetAvailableVehicles(GetAvailableVehiclesQueryHandler handler, CancellationToken token)
    {
        var vehicles = await handler.Handle(new ListVehiclesQueryDto(), token);
        return Results.Ok(vehicles);
    }

    /// <summary>
    /// Creates a new vehicle.
    /// </summary>
    /// <param name="request">Vehicle creation payload.</param>
    /// <param name="handler">Command handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result representing creation outcome.</returns>
    private static async Task<IResult> CreateVehicle(
            CreateVehicleRequest request,
            CreateVehicleHandler handler,
            CancellationToken token)
    {
        var validationErrors = EndpointRequestValidator.Validate(request);
        if (validationErrors is not null)
            return Results.ValidationProblem(validationErrors);

        var cmd = new CreateVehicleCommandDto(
            request.LicensePlate,
            request.Brand,
            request.Model,
            request.ManufactureDateUtc);

        var result = await handler.HandleAsync(cmd, DateTime.UtcNow, token);
        return Results.Created($"/vehicles/{result.Id}", result);
    }
}
