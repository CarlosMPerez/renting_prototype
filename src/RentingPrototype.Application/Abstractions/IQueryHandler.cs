namespace RentingPrototype.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult?> Handle(TQuery query, CancellationToken token);
}
