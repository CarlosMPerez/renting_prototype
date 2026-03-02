
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Vehicles.CreateVehicle;
using RentingPrototype.Application.Vehicles.Ports;
using RentingPrototype.Domain.Vehicles;

namespace RentingPrototype.Application.Vehicles;

public sealed class CreateVehicleHandler
{
    private readonly IVehicleRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateVehicleHandler(IVehicleRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<CreateVehicleResult> HandleAsync(CreateVehicleCommand cmd, DateTime nowUtc, CancellationToken token)
    {
        var vehicle = Vehicle.Create(
            id: Guid.NewGuid(),
            licensePlate: cmd.LicensePlate,
            make: cmd.Make,
            model: cmd.Model,
            manufacturingDateUtc: cmd.ManufacturingDateUtc,
            nowUtc: nowUtc);

        await _uow.BeginAsync(token);
        try
        {
            await _repo.AddAsync(vehicle, token);
            await _uow.CommitAsync(token);
            return new CreateVehicleResult(vehicle.Id);
        }
        catch
        {
            await _uow.RollbackAsync(token);
            throw;
        }
    }
}
