using System.Text.RegularExpressions;
using RentingPrototype.Domain.Common.Exceptions;

namespace RentingPrototype.Domain.VehicleDomain.ValueObjects;

public sealed record LicensePlate
{
    private static readonly Regex ValidFormat = new(
        "^[A-Z0-9-]{4,12}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    public string Value { get; }

    private LicensePlate(string value)
    {
        Value = value;
    }

    public static LicensePlate Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException("License plate cannot be empty.");
        if (value.Length > 20)
            throw new DomainValidationException("License plate max length is 20.");

        var normalized = value.Trim().ToUpperInvariant();
        if (!ValidFormat.IsMatch(normalized))
            throw new DomainValidationException("License plate format is invalid.");

        return new LicensePlate(normalized);
    }

    public override string ToString() => Value;
}
