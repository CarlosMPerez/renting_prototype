using System.Globalization;

namespace RentingPrototype.Application.Vehicle.Queries;

public sealed record VehicleQueryResultDto(
    Guid Id,
    string LicensePlate,
    string Brand,
    string Model,
    DateOnly ManufactureDate
    )
{
    /// <summary>
    /// Materialization constructor used by Dapper when SQLite returns text values.
    /// </summary>
    /// <param name="Id">Vehicle identifier as string.</param>
    /// <param name="LicensePlate">Vehicle license plate.</param>
    /// <param name="Brand">Vehicle brand.</param>
    /// <param name="Model">Vehicle model.</param>
    /// <param name="ManufactureDate">Manufacture date as string.</param>
    public VehicleQueryResultDto(
        string Id,
        string LicensePlate,
        string Brand,
        string Model,
        string ManufactureDate)
        : this(
            Guid.Parse(Id),
            LicensePlate,
            Brand,
            Model,
            ParseDate(ManufactureDate))
    {
    }

    /// <summary>
    /// Parses an incoming SQLite date representation to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="value">Date value to parse.</param>
    /// <returns>The parsed date.</returns>
    private static DateOnly ParseDate(string value)
    {
        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
            return dateOnly;

        var dateTime = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        return DateOnly.FromDateTime(dateTime);
    }
}
