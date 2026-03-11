using RentingPrototype.Domain.Common;

namespace RentingPrototype.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken token);
}
