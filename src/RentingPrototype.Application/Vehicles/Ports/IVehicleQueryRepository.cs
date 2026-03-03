using RentingPrototype.Application.Vehicles.Queries;

namespace RentingPrototype.Application.Vehicles.Ports;

public interface IVehicleQueryRepository
{
    Task<VehicleQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token);

    Task<IReadOnlyList<VehicleQueryResultDto>> GetAllAsync(CancellationToken token);

    Task<IReadOnlyList<VehicleQueryResultDto>> GetAvailableAsync(CancellationToken token);
}
