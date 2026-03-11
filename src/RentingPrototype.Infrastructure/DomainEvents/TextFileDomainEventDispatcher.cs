using System.Text;
using System.Text.Json;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Domain.Common;

namespace RentingPrototype.Infrastructure.DomainEvents;

public sealed class TextFileDomainEventDispatcher : IDomainEventDispatcher
{
    private static readonly SemaphoreSlim WriteLock = new(1, 1);
    private readonly string _logFilePath;

    public TextFileDomainEventDispatcher(string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
            throw new ArgumentException("Log file path is required.", nameof(logFilePath));

        _logFilePath = logFilePath;
    }

    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken token)
    {
        if (domainEvents.Count == 0)
            return;

        var logDirectory = Path.GetDirectoryName(_logFilePath)
            ?? throw new InvalidOperationException("Cannot resolve log directory.");

        Directory.CreateDirectory(logDirectory);

        var buffer = new StringBuilder();
        foreach (var domainEvent in domainEvents)
        {
            var payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
            buffer.Append(DateTime.UtcNow.ToString("O"));
            buffer.Append(" | ");
            buffer.Append(domainEvent.GetType().Name);
            buffer.Append(" | ");
            buffer.Append(payload);
            buffer.AppendLine();
        }

        await WriteLock.WaitAsync(token);
        try
        {
            await using var stream = new FileStream(
                _logFilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.ReadWrite,
                bufferSize: 4096,
                useAsync: true);

            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(buffer.ToString().AsMemory(), token);
            await writer.FlushAsync(token);
        }
        finally
        {
            WriteLock.Release();
        }
    }
}
