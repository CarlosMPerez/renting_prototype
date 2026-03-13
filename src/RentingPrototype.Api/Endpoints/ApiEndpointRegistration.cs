using Microsoft.AspNetCore.Routing;
using RentingPrototype.Api.Endpoints.Rental;
using RentingPrototype.Api.Endpoints.RentalHistory;
using RentingPrototype.Api.Endpoints.Vehicle;

namespace RentingPrototype.Api.Endpoints;

public static class ApiEndpointRegistration
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapVehicleEndpoints();
        app.MapRentalsEndpoints();
        app.MapRentalHistoryEndpoints();

        return app;
    }
}
