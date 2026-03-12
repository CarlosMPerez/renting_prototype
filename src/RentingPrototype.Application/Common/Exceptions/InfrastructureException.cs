namespace RentingPrototype.Application.Common.Exceptions;

public sealed class InfrastructureException : Exception
{
    public InfrastructureException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
