namespace RentingPrototype.Domain.Common.Exceptions;

public sealed class BusinessRuleViolationException : Exception
{
    public BusinessRuleViolationException(string message)
        : base(message)
    {
    }
}
