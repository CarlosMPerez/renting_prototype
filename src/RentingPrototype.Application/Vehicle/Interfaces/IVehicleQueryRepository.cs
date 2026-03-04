using RentingPrototype.Application.Vehicle.Queries;

namespace RentingPrototype.Application.Vehicle.Interfaces;

public interface IVehicleQueryRepository
{
    Task<VehicleQueryResultDto?> GetByIdAsync(Guid id, CancellationToken token);

    Task<IReadOnlyList<VehicleQueryResultDto>> GetAllAsync(CancellationToken token);

    Task<IReadOnlyList<VehicleQueryResultDto>> GetAvailableAsync(CancellationToken token);
}
