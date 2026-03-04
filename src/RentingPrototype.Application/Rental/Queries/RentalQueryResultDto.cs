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

    private static DateTime ParseDate(string value)
    {
        return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }
}