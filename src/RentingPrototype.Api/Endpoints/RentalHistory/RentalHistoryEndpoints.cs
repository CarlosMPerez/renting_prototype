using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;

namespace RentingPrototype.Api.Endpoints.RentalHistory;

public static class RentalHistoryEndpoints
{
    /// <summary>
    /// Registers rental history endpoints.
    /// </summary>
    /// <param name="app">Application route builder.</param>
    /// <returns>The configured route group.</returns>
    public static IEndpointRouteBuilder MapRentalHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rentalhistory").WithTags("RentalHistory");

        group.MapGet("/vehicles/{id:guid}/rental-history", GetVehicleRentalHistoryById).WithName("Vehicle Rental Histyory");

        group.MapGet("/customers/{id:guid}/rental-history", GetCustomerRentalHistoryById).WithName("Customer Rental Histyory");

        return group;
    }

    /// <summary>
    /// Returns rental history for a vehicle.
    /// </summary>
    /// <param name="id">Vehicle identifier.</param>
    /// <param name="handler">Query handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result containing vehicle rental history.</returns>
    private static async Task<IResult> GetVehicleRentalHistoryById(Guid id, VehicleRentalHistoryQueryHandler handler, CancellationToken token)
    {
        var rentalHistory = await handler.Handle(new VehicleRentalHistoryFilterDto(id), token);
        return rentalHistory is null ? Results.NotFound() : Results.Ok(rentalHistory);
    }

    /// <summary>
    /// Returns rental history for a customer.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="handler">Query handler.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>HTTP result containing customer rental history.</returns>
    private static async Task<IResult> GetCustomerRentalHistoryById(Guid id, CustomerRentalHistoryQueryHandler handler, CancellationToken token)
    {
        var rentalHistory = await handler.Handle(new CustomerRentalHistoryFilterDto(id), token);
        return rentalHistory is null ? Results.NotFound() : Results.Ok(rentalHistory);
    }
}
