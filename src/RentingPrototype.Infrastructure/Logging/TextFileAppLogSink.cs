using System.Text.Json;
using RentingPrototype.Application.Abstractions;

namespace RentingPrototype.Infrastructure.Logging;

public sealed class TextFileAppLogSink : IAppLogSink
{
    private static readonly SemaphoreSlim WriteLock = new(1, 1);
    private readonly string _logFilePath;

    public TextFileAppLogSink(string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
            throw new ArgumentException("Log file path is required.", nameof(logFilePath));

        _logFilePath = logFilePath;
    }

    public async Task WriteAsync(AppLogRecord record, CancellationToken token)
    {
        var logDirectory = Path.GetDirectoryName(_logFilePath)
            ?? throw new InvalidOperationException("Cannot resolve log directory.");

        Directory.CreateDirectory(logDirectory);

        var logLine = JsonSerializer.Serialize(record);

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
            await writer.WriteLineAsync(logLine.AsMemory(), token);
            await writer.FlushAsync(token);
        }
        finally
        {
            WriteLock.Release();
        }
    }
}
