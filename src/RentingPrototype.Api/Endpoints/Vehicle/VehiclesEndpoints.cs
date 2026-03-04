using Microsoft.Data.Sqlite;
using RentingPrototype.Application.Vehicle.Commands;
using RentingPrototype.Application.Vehicle.Queries;

namespace RentingPrototype.Api.Endpoints.Vehicle;

public static class VehicleEndpoints
{
    public static RouteGroupBuilder MapVehicleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/vehicles").WithTags("Vehicles");

        // TO-DO hacer que los endpoints de lectura sean tan elaborados como el de escritura, y crearlos en su propio método privado también

        // GET/vehicles/{id}
        group.MapGet("/{id:guid}", async (Guid id, GetVehicleByIdQueryHandler handler, CancellationToken token)
        =>
        {
            var vehicle = await handler.Handle(new VehicleQueryFilterDto(id), token);
            return vehicle is null ? Results.NotFound() : Results.Ok(vehicle);
        }).WithName("GetVehicleById");

        // GET /vehicles
        group.MapGet("/", async (GetAllVehiclesQueryHandler handler, CancellationToken token)
        =>
        {
            var vehicles = await handler.Handle(new ListVehiclesQueryDto(), token);
            return Results.Ok(vehicles);
        }).WithName("GetAllVehicles");

        // GET /vehicles/available
        group.MapGet("/available", async (GetAvailableVehiclesQueryHandler handler, CancellationToken token)
        =>
        {
            var vehicles = await handler.Handle(new ListVehiclesQueryDto(), token);
            return Results.Ok(vehicles);
        }).WithName("GetAvailableVehicles");

        // POST 
        group.MapPost("/", CreateVehicle);

        return group;
    }

    private static async Task<IResult> CreateVehicle(
            CreateVehicleCommandDto request,
            CreateVehicleHandler handler,
            CancellationToken token)
    {
        try
        {
            var cmd = new CreateVehicleCommandDto(request.LicensePlate,
                request.Brand, request.Model, request.ManufactureDateUtc);
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
