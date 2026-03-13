using System.Globalization;

namespace RentingPrototype.Application.RentalHistory.Queries.CustomerRentalHistory;

public sealed record CustomerRentalHistoryResultDto(
    Guid VehicleId,
    string LicensePlate,
    string Brand,
    string Model,
    DateOnly StartDate,
    DateOnly? EndDate)
{
    /// <summary>
    /// Materialization constructor used by Dapper when Sqlite returns text values.
    /// </summary>
    /// <param name="VehicleId">Vehicle identifier as string.</param>
    /// <param name="LicensePlate">Vehicle license plate.</param>
    /// <param name="Brand">Vehicle brand.</param>
    /// <param name="Model">Vehicle model.</param>
    /// <param name="StartDate">Rental start date as string.</param>
    /// <param name="EndDate">Rental end date as string, when present.</param>
    public CustomerRentalHistoryResultDto(
        string VehicleId,
        string LicensePlate,
        string Brand,
        string Model,
        string StartDate,
        string? EndDate) : this(
            Guid.Parse(VehicleId),
            LicensePlate,
            Brand,
            Model,
            ParseDate(StartDate),
            EndDate != null ? ParseDate(EndDate) : null)
    {
    }

    /// <summary>
    /// Parses a Sqlite date representation to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="value">Date value to parse.</param>
    /// <returns>The parsed date.</returns>
    private static DateOnly ParseDate(string value)
    {
        return DateOnly.Parse(value, CultureInfo.InvariantCulture);
    }
}
