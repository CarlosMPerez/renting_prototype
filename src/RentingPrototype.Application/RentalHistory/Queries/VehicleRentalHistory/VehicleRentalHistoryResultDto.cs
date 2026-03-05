using System.Globalization;

namespace RentingPrototype.Application.RentalHistory.Queries.VehicleRentalHistory;

public sealed record VehicleRentalHistoryResultDto(
    Guid CustomerId,
    string DocumentId,
    string Name,
    string Surname,
    DateOnly StartDate,
    DateOnly? EndDate)
{
    /// <summary>
    /// Materialization constructor used by Dapper when SQLite returns text values.
    /// </summary>
    /// <param name="CustomerId">Customer identifier as string.</param>
    /// <param name="DocumentId">Customer document id.</param>
    /// <param name="Name">Customer first name.</param>
    /// <param name="Surname">Customer surname.</param>
    /// <param name="StartDate">Rental start date as string.</param>
    /// <param name="EndDate">Rental end date as string, when present.</param>
    public VehicleRentalHistoryResultDto(
        string CustomerId,
        string DocumentId,
        string Name,
        string Surname,
        string StartDate,
        string? EndDate) : this(
            Guid.Parse(CustomerId),
            DocumentId,
            Name,
            Surname,
            ParseDate(StartDate),
            EndDate != null ? ParseDate(EndDate) : null)
    {

    }

    /// <summary>
    /// Parses a SQLite date representation to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="value">Date value to parse.</param>
    /// <returns>The parsed date.</returns>
    private static DateOnly ParseDate(string value)
    {
        return DateOnly.Parse(value, CultureInfo.InvariantCulture);
    }
}
