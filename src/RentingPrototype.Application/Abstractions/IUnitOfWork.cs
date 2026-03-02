using System;

namespace RentingPrototype.Application.Abstractions;

public interface IUnitOfWork
{
    Task BeginAsync(CancellationToken token);
    Task CommitAsync(CancellationToken token);
    Task RollbackAsync(CancellationToken token);
}
