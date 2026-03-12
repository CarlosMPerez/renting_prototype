using RentingPrototype.Domain.Common.Exceptions;

namespace RentingPrototype.Domain.VehicleDomain.ValueObjects;

public sealed record ManufactureDateOnly
{
    public DateOnly Value { get; }

    private ManufactureDateOnly(DateOnly value)
    {
        Value = value;
    }

    public static ManufactureDateOnly Create(DateOnly value, DateOnly nowUtc)
    {
        if (!IsValidAge(value, nowUtc))
            throw new BusinessRuleViolationException("Vehicle is older than 5 years and cannot be registered.");

        return new ManufactureDateOnly(value);
    }

    public static ManufactureDateOnly Create(DateTime value, DateTime nowUtc)
    {
        var convValue = DateOnly.FromDateTime(value);
        var convNow = DateOnly.FromDateTime(nowUtc);
        if (!IsValidAge(convValue, convNow))
            throw new BusinessRuleViolationException("Vehicle is older than 5 years and cannot be registered.");

        return new ManufactureDateOnly(convValue);
    }
    public override string ToString() => Value.ToString("O");

    private static bool IsValidAge(DateOnly val, DateOnly nowUtc)
    {
        var minDate = nowUtc.AddYears(-5);
        return val >= minDate;
    }
}
