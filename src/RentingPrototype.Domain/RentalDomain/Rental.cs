namespace RentingPrototype.Domain.RentalDomain;

public sealed class Rental
{
    public Guid Id { get; }
    public Guid CustomerId { get; }
    public Guid VehicleId { get; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }

    /// <summary>
    /// Initializes a rental aggregate instance.
    /// </summary>
    private Rental(Guid id, Guid customerId, Guid vehicleId, DateTime startDate, DateTime? endDate = null)
    {
        Id = id;
        CustomerId = customerId;
        VehicleId = vehicleId;
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Creates a rental aggregate enforcing the domain invariants.
    /// </summary>
    /// <param name="id">Rental identifier.</param>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="startDate">Rental start date.</param>
    /// <param name="endDate">Rental end date when available.</param>
    /// <returns>A valid <see cref="Rental"/> instance.</returns>
    public static Rental Create(Guid id, Guid customerId, Guid vehicleId, DateTime startDate, DateTime? endDate = null)
    {
        if (customerId == Guid.Empty) throw new ArgumentException("Customer Id cannot be empty.", nameof(customerId));
        if (vehicleId == Guid.Empty) throw new ArgumentException("Vehicle Id cannot be empty.", nameof(vehicleId));
        if (startDate.Date > DateTime.UtcNow.Date) throw new ArgumentException("Cannot start a rental in the future", nameof(startDate));
        if (endDate != null && startDate.Date > endDate.Value.Date) throw new ArgumentException("End Date cannot be before Start Date", nameof(endDate));

        return new Rental(
            id,
            customerId,
            vehicleId,
            startDate,
            endDate);
    }

}
