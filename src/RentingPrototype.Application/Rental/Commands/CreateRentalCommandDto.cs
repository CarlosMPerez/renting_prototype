namespace RentingPrototype.Application.Rental.Commands;

public record class CreateRentalCommandDto(
    Guid CustomerId,
    Guid VehicleId,
    DateTime StartDate,
    DateTime? EndDate = null
);
