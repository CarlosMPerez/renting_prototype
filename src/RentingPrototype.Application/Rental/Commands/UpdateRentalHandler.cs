using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Common.Exceptions;
using RentingPrototype.Application.Rental.Ports;
using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Application.Rental.Commands;

public sealed record UpdateRentalResultDto(Guid Id);

public sealed class UpdateRentalHandler
{
    private readonly IRentalCommandRepository _commandRepo;
    private readonly IRentalQueryRepository _queryRepo;

    private readonly IUnitOfWork _uow;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    /// <summary>
    /// Creates a handler instance for rental updates.
    /// </summary>
    /// <param name="commandRepo">Rental command repository.</param>
    /// <param name="queryRepo">Rental query repository.</param>
    /// <param name="uow">Unit of work.</param>
    /// <param name="domainEventDispatcher">Domain event dispatcher.</param>
    public UpdateRentalHandler(
        IRentalCommandRepository commandRepo,
        IRentalQueryRepository queryRepo,
        IUnitOfWork uow,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _commandRepo = commandRepo;
        _queryRepo = queryRepo;
        _uow = uow;
        _domainEventDispatcher = domainEventDispatcher;
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
                ?? throw new NotFoundException($"Rental '{dto.Id}' not found.");

            var rentalToUpdate = RentalDomain.Rental.Rehydrate(
                dto.Id,
                rental.CustomerId,
                rental.VehicleId,
                rental.StartDate,
                rental.EndDate);

            rentalToUpdate.Return(dto.EndDate);

            await _commandRepo.UpdateAsync(rentalToUpdate, token);
            await _uow.CommitAsync(token);
            await _domainEventDispatcher.DispatchAsync(rentalToUpdate.PullDomainEvents(), token);
            return new UpdateRentalResultDto(dto.Id);
        }
        catch
        {
            await _uow.RollbackAsync(token);
            throw;
        }
    }
}
