using Microsoft.Data.Sqlite;
using RentingPrototype.Api.Contracts;
using RentingPrototype.Application.Vehicles;
using RentingPrototype.Application.Vehicles.CreateVehicle;

namespace RentingPrototype.Api.Endpoints.Vehicles;

public static class VehiclesEndpoints
{
    public static IEndpointRouteBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/vehicles");
        group.MapPost("/", CreateVehicle);
        return app;
    }

    private static async Task<IResult> CreateVehicle(
            CreateVehicleRequest request, 
            CreateVehicleHandler handler, 
            CancellationToken token)
    {
        try
        {
            var cmd = new CreateVehicleCommand(request.LicensePlate, 
                request.Make, request.Model, request.ManufacturingDateUtc);
            var result = await handler.HandleAsync(cmd, DateTime.UtcNow, token);
            return Results.Created($"/vehicles/{result.Id}", result);
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
