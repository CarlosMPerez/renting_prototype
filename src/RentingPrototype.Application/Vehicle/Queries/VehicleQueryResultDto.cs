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
    // Dapper reads SQLite TEXT columns as string and uses constructor binding for records.
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
            ParseManufactureDate(ManufactureDate))
    {
    }

    private static DateOnly ParseManufactureDate(string value)
    {
        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
            return dateOnly;

        var dateTime = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        return DateOnly.FromDateTime(dateTime);
    }
}
