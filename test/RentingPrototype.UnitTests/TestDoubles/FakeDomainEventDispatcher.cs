using RentingPrototype.Application.Abstractions;
using RentingPrototype.Domain.Common;

namespace RentingPrototype.UnitTests.TestDoubles;

public sealed class FakeDomainEventDispatcher : IDomainEventDispatcher
{
    public List<IDomainEvent> PublishedEvents { get; } = new();

    public Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken token)
    {
        PublishedEvents.AddRange(domainEvents);
        return Task.CompletedTask;
    }
}
