namespace RentingPrototype.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResult>
{
    /// <summary>
    /// Handles a query and returns its result.
    /// </summary>
    /// <param name="query">Query input payload.</param>
    /// <param name="token">Cancellation token for the operation.</param>
    /// <returns>The query result, or <c>null</c> when not found.</returns>
    Task<TResult?> Handle(TQuery query, CancellationToken token);
}
