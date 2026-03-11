using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Rental.Ports;
using RentalDomain = RentingPrototype.Domain.RentalDomain;

namespace RentingPrototype.Application.Rental.Commands;

public sealed record CreateRentalResultDto(Guid Id);

public sealed class CreateRentalHandler
{
    private readonly IRentalCommandRepository _commandRepo;
    private readonly IRentalQueryRepository _queryRepo;
    private readonly IUnitOfWork _uow;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    /// <summary>
    /// Creates a handler instance for rental creation.
    /// </summary>
    /// <param name="commandRepo">Rental command repository.</param>
    /// <param name="queryRepo">Rental query repository.</param>
    /// <param name="uow">Unit of work.</param>
    /// <param name="domainEventDispatcher">Domain event dispatcher.</param>
    public CreateRentalHandler(
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
    /// Validates business preconditions and creates a rental transactionally.
    /// </summary>
    /// <param name="dto">Rental input payload.</param>
    /// <param name="nowUtc">Current UTC date (reserved for time-based rules).</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The identifier of the created rental.</returns>
    public async Task<CreateRentalResultDto> HandleAsync(CreateRentalCommandDto dto, DateTime nowUtc, CancellationToken token)
    {
        if (await _queryRepo.HasOpenRentalByCustomerAsync(dto.CustomerId, token))
            throw new InvalidOperationException("Customer already has an active rental.");
        if (await _queryRepo.HasOpenRentalByVehicleAsync(dto.VehicleId, token))
            throw new InvalidOperationException("Vehicle already has an active rental.");

        var rental = RentalDomain.Rental.Create(
            id: Guid.NewGuid(),
            customerId: dto.CustomerId,
            vehicleId: dto.VehicleId,
            startDate: dto.StartDate);

        await _uow.BeginAsync(token);
        try
        {
            await _commandRepo.CreateAsync(rental, token);
            await _uow.CommitAsync(token);
            await _domainEventDispatcher.DispatchAsync(rental.PullDomainEvents(), token);
            return new CreateRentalResultDto(rental.Id);
        }
        catch
        {
            await _uow.RollbackAsync(token);
            throw;
        }
    }
}
