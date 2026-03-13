using System.Globalization;

namespace RentingPrototype.Application.Rental.Queries;

public sealed record RentalQueryResultDto(
    Guid Id,
    Guid CustomerId,
    Guid VehicleId,
    DateTime StartDate,
    DateTime? EndDate
    )
{
    /// <summary>
    /// Materialization constructor used by Dapper when Sqlite returns text values.
    /// </summary>
    /// <param name="id">Rental identifier as string.</param>
    /// <param name="customerId">Customer identifier as string.</param>
    /// <param name="vehicleId">Vehicle identifier as string.</param>
    /// <param name="startDate">Start date as string.</param>
    /// <param name="endDate">Optional end date as string.</param>
    public RentalQueryResultDto(
        string id,
        string customerId,
        string vehicleId,
        string startDate,
        string? endDate)
        : this(
            Guid.Parse(id),
            Guid.Parse(customerId),
            Guid.Parse(vehicleId),
            ParseDate(startDate),
            endDate != null ? ParseDate(endDate) : null)
    {
    }

    /// <summary>
    /// Parses a Sqlite date representation to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="value">Date value to parse.</param>
    /// <returns>The parsed date.</returns>
    private static DateTime ParseDate(string value)
    {
        return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }
}
