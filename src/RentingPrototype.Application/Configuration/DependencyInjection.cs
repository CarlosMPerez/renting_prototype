using Microsoft.Extensions.DependencyInjection;
using RentingPrototype.Application.Rental.Commands;
using RentingPrototype.Application.Rental.Queries;
using RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;
using RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;
using RentingPrototype.Application.Vehicle.Commands;
using RentingPrototype.Application.Vehicle.Queries;

namespace RentingPrototype.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateVehicleHandler>();
        services.AddScoped<GetVehicleByIdQueryHandler>();
        services.AddScoped<GetAllVehiclesQueryHandler>();
        services.AddScoped<GetAvailableVehiclesQueryHandler>();

        services.AddScoped<CreateRentalHandler>();
        services.AddScoped<UpdateRentalHandler>();
        services.AddScoped<GetRentalByIdQueryHandler>();

        services.AddScoped<VehicleRentalHistoryQueryHandler>();
        services.AddScoped<CustomerRentalHistoryQueryHandler>();

        return services;
    }
}