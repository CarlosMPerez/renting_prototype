using System;

namespace RentingPrototype.Application.Abstractions;

public interface IUnitOfWork
{
    /// <summary>
    /// Starts a new transaction scope.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    Task BeginAsync(CancellationToken token);
    /// <summary>
    /// Commits the current transaction scope.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    Task CommitAsync(CancellationToken token);
    /// <summary>
    /// Rolls back the current transaction scope.
    /// </summary>
    /// <param name="token">Cancellation token for the operation.</param>
    Task RollbackAsync(CancellationToken token);
}
