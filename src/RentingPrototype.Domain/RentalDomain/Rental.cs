namespace RentingPrototype.Domain.RentalDomain;

public sealed class Rental
{
    public Guid Id { get; }
    public Guid CustomerId { get; }
    public Guid VehicleId { get; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }

    private Rental(Guid id, Guid customerId, Guid vehicleId, DateTime startDate, DateTime? endDate = null)
    {
        Id = id;
        CustomerId = customerId;
        VehicleId = vehicleId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Rental Create(Guid id, Guid customerId, Guid vehicleId, DateTime startDate, DateTime? endDate = null)
    {
        if (customerId == Guid.Empty) throw new ArgumentException("Customer Id cannot be empty.", nameof(customerId));
        if (vehicleId == Guid.Empty) throw new ArgumentException("Vehicle Id cannot be empty.", nameof(vehicleId));
        // TO-DO Validar que tanto customerId como vehicleId existen en la BDD Es Aqui ??

        return new Rental(
            id,
            customerId,
            vehicleId,
            startDate,
            endDate);
    }

}