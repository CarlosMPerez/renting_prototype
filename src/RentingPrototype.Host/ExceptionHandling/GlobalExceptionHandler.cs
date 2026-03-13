using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using RentingPrototype.Application.Abstractions;
using RentingPrototype.Application.Common.Exceptions;
using RentingPrototype.Domain.Common.Exceptions;

namespace RentingPrototype.Host.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IAppLogSink _appLogSink;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IAppLogSink appLogSink)
    {
        _logger = logger;
        _appLogSink = appLogSink;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken token)
    {
        var (statusCode, title, detail, type) = MapException(exception);
        var traceId = httpContext.TraceIdentifier;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception,
                "Unhandled exception in {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);
        }
        else
        {
            _logger.LogWarning(exception,
                "Handled exception in {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);
        }

        try
        {
            await _appLogSink.WriteAsync(new AppLogRecord(
                TimestampUtc: DateTime.UtcNow,
                Type: "exception",
                Name: exception.GetType().Name,
                Message: exception.Message,
                TraceId: traceId,
                Path: httpContext.Request.Path,
                Data: new
                {
                    statusCode,
                    method = httpContext.Request.Method
                },
                StackTrace: ShouldIncludeStackTrace(exception) ? exception.StackTrace : null), token);
        }
        catch (Exception sinkException)
        {
            _logger.LogError(sinkException, "Failed to write exception to app log sink.");
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = httpContext.Request.Path
        };
        problem.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, token);
        return true;
    }

    private static (int StatusCode, string Title, string Detail, string Type) MapException(Exception exception)
    {
        return exception switch
        {
            DomainValidationException ex => (
                StatusCodes.Status400BadRequest,
                "Domain validation failed",
                ex.Message,
                "https://httpstatuses.com/400"),

            BusinessRuleViolationException ex => (
                StatusCodes.Status409Conflict,
                "Business rule violated",
                ex.Message,
                "https://httpstatuses.com/409"),

            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                ex.Message,
                "https://httpstatuses.com/404"),

            KeyNotFoundException ex => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                ex.Message,
                "https://httpstatuses.com/404"),

            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                ex.Message,
                "https://httpstatuses.com/400"),

            JsonException => (
                StatusCodes.Status400BadRequest,
                "Invalid JSON payload",
                "The request body contains invalid JSON.",
                "https://httpstatuses.com/400"),

            BadHttpRequestException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid HTTP request",
                ex.Message,
                "https://httpstatuses.com/400"),

            SqliteException sqliteEx when sqliteEx.SqliteErrorCode == 19 => (
                StatusCodes.Status409Conflict,
                "Database constraint violation",
                "The operation conflicts with a database constraint.",
                "https://httpstatuses.com/409"),

            SqliteException => (
                StatusCodes.Status503ServiceUnavailable,
                "Database unavailable",
                "The database operation failed.",
                "https://httpstatuses.com/503"),

            InfrastructureException => (
                StatusCodes.Status503ServiceUnavailable,
                "Infrastructure error",
                "An infrastructure dependency failed.",
                "https://httpstatuses.com/503"),

            OperationCanceledException => (
                StatusCodes.Status400BadRequest,
                "Request cancelled",
                "The request was cancelled.",
                "https://httpstatuses.com/400"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred.",
                "https://httpstatuses.com/500")
        };
    }

    private static bool ShouldIncludeStackTrace(Exception exception)
    {
        var exceptionNamespace = exception.GetType().Namespace;
        return string.IsNullOrWhiteSpace(exceptionNamespace)
            || !exceptionNamespace.StartsWith("RentingPrototype.", StringComparison.Ordinal);
    }
}
