using Microsoft.Data.Sqlite;
using RentingPrototype.Application.Rental.Commands;

namespace RentingPrototype.Api.Endpoints.Rental;

public static class RentalEndpoints
{
    public static RouteGroupBuilder MapRentalsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rentals").WithTags("Rentals");

        // POST 
        group.MapPost("/rent-vehicle", RentVehicle);

        group.MapPost("/return-vehicle", ReturnVehicle);

        return group;
    }

    private static async Task<IResult> RentVehicle(
            CreateRentalCommandDto request,
            CreateRentalHandler handler,
            CancellationToken token)
    {
        try
        {
            var cmd = new CreateRentalCommandDto(request.CustomerId,
                request.VehicleId, request.StartDate);
            var result = await handler.HandleAsync(cmd, DateTime.UtcNow, token);
            return Results.Created($"/rentals/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return Results.Conflict(new { error = $"Constraint violation. {ex.Message}" });
        }
    }

    private static async Task<IResult> ReturnVehicle(
            UpdateRentalCommandDto request,
            UpdateRentalHandler handler,
            CancellationToken token)
    {
        try
        {
            var cmd = new UpdateRentalCommandDto(request.Id, request.EndDate);
            var result = await handler.HandleAsync(cmd, DateTime.UtcNow, token);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return Results.Conflict(new { error = $"Constraint violation. {ex.Message}" });
        }
    }
}
