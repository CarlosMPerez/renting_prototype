using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Ports;
using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Application.Rental.Commands;

public sealed record UpdateRentalResultDto(Guid Id);

public sealed class UpdateRentalHandler
{
    private readonly IRentalCommandRepository _commandRepo;
    private readonly IRentalQueryRepository _queryRepo;

    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Creates a handler instance for rental updates.
    /// </summary>
    /// <param name="commandRepo">Rental command repository.</param>
    /// <param name="queryRepo">Rental query repository.</param>
    /// <param name="uow">Unit of work.</param>
    public UpdateRentalHandler(IRentalCommandRepository commandRepo, IRentalQueryRepository queryRepo, IUnitOfWork uow)
    {
        _commandRepo = commandRepo;
        _queryRepo = queryRepo;
        _uow = uow;
    }

    /// <summary>
    /// Closes an existing rental by setting its end date.
    /// </summary>
    /// <param name="dto">Rental update payload.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The identifier of the updated rental.</returns>
    public async Task<UpdateRentalResultDto> HandleAsync(UpdateRentalCommandDto dto, CancellationToken token)
    {
        await _uow.BeginAsync(token);
        try
        {
            var rental = await _queryRepo.GetByIdAsync(dto.Id, token)
                ?? throw new KeyNotFoundException($"Rental '{dto.Id}' not found.");

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
