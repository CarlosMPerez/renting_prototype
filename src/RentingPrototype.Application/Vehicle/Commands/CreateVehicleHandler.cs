using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Interfaces;
using VehicleDomain = RentingPrototype.Domain.VehicleDomain;

namespace RentingPrototype.Application.Vehicle.Commands;

public sealed record CreateVehicleResultDto(Guid Id);

public sealed class CreateVehicleHandler
{
    private readonly IVehicleCommandRepository _repo;
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Creates a handler instance for vehicle creation.
    /// </summary>
    /// <param name="repo">Vehicle command repository.</param>
    /// <param name="uow">Unit of work.</param>
    public CreateVehicleHandler(IVehicleCommandRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    /// <summary>
    /// Validates and creates a new vehicle in a transactional scope.
    /// </summary>
    /// <param name="dto">Vehicle input payload.</param>
    /// <param name="nowUtc">Current UTC date used for domain validations.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The identifier of the created vehicle.</returns>
    public async Task<CreateVehicleResultDto> HandleAsync(CreateVehicleCommandDto dto, DateTime nowUtc, CancellationToken token)
    {
        var vehicle = VehicleDomain.Vehicle.Create(
            id: Guid.NewGuid(),
            licensePlate: dto.LicensePlate,
            brand: dto.Brand,
            model: dto.Model,
            manufactureDateUtc: dto.ManufactureDateUtc,
            nowUtc: nowUtc);

        await _uow.BeginAsync(token);
        try
        {
            await _repo.AddAsync(vehicle, token);
            await _uow.CommitAsync(token);
            return new CreateVehicleResultDto(vehicle.Id);
        }
        catch
        {
            await _uow.RollbackAsync(token);
            throw;
        }
    }
}
