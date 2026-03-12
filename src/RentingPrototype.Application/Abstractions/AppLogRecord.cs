namespace RentingPrototype.Application.Abstractions;

public sealed record AppLogRecord(
    DateTime TimestampUtc,
    string Type,
    string Name,
    string Message,
    string? TraceId = null,
    string? Path = null,
    object? Data = null,
    string? StackTrace = null);
