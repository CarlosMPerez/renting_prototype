using RentingPrototype.Domain.Common;
using RentingPrototype.Domain.Common.Exceptions;
using RentingPrototype.Domain.RentalDomain.Events;

namespace RentingPrototype.Domain.RentalDomain;

public sealed class Rental : AggregateRoot
{
    public Guid Id { get; }
    public Guid CustomerId { get; }
    public Guid VehicleId { get; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; private set; }

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
        Validate(customerId, vehicleId, startDate, endDate);

        var rental = new Rental(
            id,
            customerId,
            vehicleId,
            startDate,
            endDate);

        rental.AddDomainEvent(new VehicleRentedDomainEvent(
            rental.Id,
            rental.VehicleId,
            rental.CustomerId,
            rental.StartDate,
            DateTime.UtcNow));

        return rental;
    }

    /// <summary>
    /// Rehydrates a rental aggregate from persistence without emitting new domain events.
    /// </summary>
    /// <param name="id">Rental identifier.</param>
    /// <param name="customerId">Customer identifier.</param>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="startDate">Rental start date.</param>
    /// <param name="endDate">Rental end date when available.</param>
    /// <returns>A rental aggregate with current persisted state.</returns>
    public static Rental Rehydrate(Guid id, Guid customerId, Guid vehicleId, DateTime startDate, DateTime? endDate)
    {
        Validate(customerId, vehicleId, startDate, endDate);
        return new Rental(id, customerId, vehicleId, startDate, endDate);
    }

    /// <summary>
    /// Marks the rental as returned and emits a domain event.
    /// </summary>
    /// <param name="endDate">Return date.</param>
    public void Return(DateTime endDate)
    {
        if (EndDate is not null)
            throw new BusinessRuleViolationException("Rental has already been returned.");
        if (endDate.Date < StartDate.Date)
            throw new DomainValidationException("End Date cannot be before Start Date.");

        EndDate = endDate;

        AddDomainEvent(new VehicleReturnedDomainEvent(
            Id,
            VehicleId,
            CustomerId,
            EndDate.Value,
            DateTime.UtcNow));
    }

    private static void Validate(Guid customerId, Guid vehicleId, DateTime startDate, DateTime? endDate)
    {
        if (customerId == Guid.Empty) throw new DomainValidationException("Customer Id cannot be empty.");
        if (vehicleId == Guid.Empty) throw new DomainValidationException("Vehicle Id cannot be empty.");
        if (startDate.Date > DateTime.UtcNow.Date) throw new DomainValidationException("Cannot start a rental in the future.");
        if (endDate != null && startDate.Date > endDate.Value.Date) throw new DomainValidationException("End Date cannot be before Start Date.");
    }
}
