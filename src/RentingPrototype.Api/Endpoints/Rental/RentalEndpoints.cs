using RentingPrototype.Api.Contracts.Rental;
using RentingPrototype.Api.Validation;
using RentingPrototype.Application.Rental.Commands;

namespace RentingPrototype.Api.Endpoints.Rental;

public static class RentalEndpoints
{
    /// <summary>
    /// Registers rental endpoints.
    /// </summary>
    /// <param name="app">Application route builder.</param>
    /// <returns>The configured route group.</returns>
    public static RouteGroupBuilder MapRentalsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rentals").WithTags("Rentals");

        group.MapPost("/rent-vehicle", RentVehicle).WithName("Rent a vehicle");

        group.MapPost("/return-vehicle", ReturnVehicle).WithName("Return a vehicle");

        return group;
    }

    /// <summary>
    /// Creates a rental for a customer and vehicle.
    /// </summary>
    /// <param name="request">Rental creation payload.</param>
    /// <param name="handler">Command handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result representing creation outcome.</returns>
    private static async Task<IResult> RentVehicle(
            CreateRentalRequest request,
            CreateRentalHandler handler,
            CancellationToken token)
    {
        var validationErrors = EndpointRequestValidator.Validate(request);
        if (validationErrors is not null)
            return Results.ValidationProblem(validationErrors);

        var cmd = new CreateRentalCommandDto(
            request.CustomerId,
            request.VehicleId,
            request.StartDate);

        var result = await handler.HandleAsync(cmd, DateTime.UtcNow, token);
        return Results.Created($"/rentals/{result.Id}", result);
    }

    /// <summary>
    /// Closes an active rental by setting its end date.
    /// </summary>
    /// <param name="request">Rental return payload.</param>
    /// <param name="handler">Command handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result representing update outcome.</returns>
    private static async Task<IResult> ReturnVehicle(
            ReturnRentalRequest request,
            UpdateRentalHandler handler,
            CancellationToken token)
    {
        var validationErrors = EndpointRequestValidator.Validate(request);
        if (validationErrors is not null)
            return Results.ValidationProblem(validationErrors);

        var cmd = new UpdateRentalCommandDto(request.Id, request.EndDate);
        var result = await handler.HandleAsync(cmd, token);
        return Results.Ok(result);
    }
}
