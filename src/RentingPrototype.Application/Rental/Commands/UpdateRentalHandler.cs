using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Interfaces;
using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Application.Rental.Commands;

public sealed record UpdateRentalResultDto(Guid Id);

public sealed class UpdateRentalHandler
{
    private readonly IRentalCommandRepository _commandRepo;
    private readonly IRentalQueryRepository _queryRepo;

    private readonly IUnitOfWork _uow;

    public UpdateRentalHandler(IRentalCommandRepository commandRepo, IRentalQueryRepository queryRepo, IUnitOfWork uow)
    {
        _commandRepo = commandRepo;
        _queryRepo = queryRepo;
        _uow = uow;
    }

    public async Task<UpdateRentalResultDto> HandleAsync(UpdateRentalCommandDto dto, DateTime nowUtc, CancellationToken token)
    {
        await _uow.BeginAsync(token);
        try
        {
            // TO-DO Y si rental es nulo aqui?
            var rental = await _queryRepo.GetByIdAsync(dto.Id, token);

            await _commandRepo.UpdateAsync(RentalDomain.Rental.Create(
                dto.Id,
                rental.CustomerId,
                rental.VehicleId,
                rental.StartDate,
                dto.EndDate), token);
            await _uow.CommitAsync(token);
            return new UpdateRentalResultDto(dto.Id);
        }
        catch
        {
            await _uow.RollbackAsync(token);
            throw;
        }
    }
}
