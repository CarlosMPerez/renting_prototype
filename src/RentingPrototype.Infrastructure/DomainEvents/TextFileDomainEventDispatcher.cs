using RentingPrototype.Application.Abstractions;
using RentingPrototype.Domain.Common;

namespace RentingPrototype.Infrastructure.DomainEvents;

public sealed class TextFileDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IAppLogSink _appLogSink;

    public TextFileDomainEventDispatcher(IAppLogSink appLogSink)
        => _appLogSink = appLogSink;

    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken token)
    {
        if (domainEvents.Count == 0)
            return;

        foreach (var domainEvent in domainEvents)
        {
            await _appLogSink.WriteAsync(new AppLogRecord(
                TimestampUtc: DateTime.UtcNow,
                Type: "domain_event",
                Name: domainEvent.GetType().Name,
                Message: "Domain event dispatched.",
                Data: domainEvent), token);
        }
    }
}
