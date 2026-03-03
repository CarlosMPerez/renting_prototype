using RentingPrototype.Application.Abstractions;

namespace RentingPrototype.UnitTests.TestDoubles;

public class FakeUnitOfWork : IUnitOfWork
{
    public bool Begun { get; private set; }
    public bool Committed { get; private set; }
    public bool RolledBack { get; private set; }

    public Task BeginAsync(CancellationToken token)
    {
        Begun = true;
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken token)
    {
        Committed = true;
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken token)
    {
        RolledBack = true;
        return Task.CompletedTask;
    }
}
