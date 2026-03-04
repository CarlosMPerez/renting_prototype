namespace RentingPrototype.Application.Rental.Commands;

public record class UpdateRentalCommandDto(
    Guid Id,
    DateTime EndDate
);
