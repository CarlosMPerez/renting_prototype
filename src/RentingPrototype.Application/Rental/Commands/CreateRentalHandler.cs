using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Interfaces;
using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Application.Rental.Commands;

public sealed record CreateRentalResultDto(Guid Id);

public sealed class CreateRentalHandler
{
    private readonly IRentalCommandRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateRentalHandler(IRentalCommandRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<CreateRentalResultDto> HandleAsync(CreateRentalCommandDto dto, DateTime nowUtc, CancellationToken token)
    {
        var rental = RentalDomain.Rental.Create(
            id: Guid.NewGuid(),
            customerId: dto.CustomerId,
            vehicleId: dto.VehicleId,
            startDate: dto.StartDate);

        await _uow.BeginAsync(token);
        try
        {
            await _repo.CreateAsync(rental, token);
            await _uow.CommitAsync(token);
            return new CreateRentalResultDto(rental.Id);
        }
        catch
        {
            await _uow.RollbackAsync(token);
            throw;
        }
    }
}
