using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicle.Interfaces;
using VehicleDomain = RentingPrototype.Domain.VehicleDomain;

namespace RentingPrototype.Application.Vehicle.Commands;

public sealed record CreateVehicleResultDto(Guid Id);

public sealed class CreateVehicleHandler
{
    private readonly IVehicleCommandRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateVehicleHandler(IVehicleCommandRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

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
