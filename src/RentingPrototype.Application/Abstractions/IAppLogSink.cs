namespace RentingPrototype.Application.Abstractions;

public interface IAppLogSink
{
    Task WriteAsync(AppLogRecord record, CancellationToken token);
}
