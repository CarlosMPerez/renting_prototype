using RentingPrototype.Domain.Common.Exceptions;
using RentingPrototype.Domain.RentalDomain;
using RentingPrototype.Domain.RentalDomain.Events;

namespace RentingPrototype.UnitTests.RentalTesting;

public sealed class RentalTests
{
    [Fact]
    public void Create_RejectsEmptyCustomerId()
    {
        var ex = Assert.Throws<DomainValidationException>(() =>
            Rental.Create(
                id: Guid.NewGuid(),
                customerId: Guid.Empty,
                vehicleId: Guid.NewGuid(),
                startDate: DateTime.UtcNow.AddDays(-1)));

        Assert.Contains("Customer Id cannot be empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsEmptyVehicleId()
    {
        var ex = Assert.Throws<DomainValidationException>(() =>
            Rental.Create(
                id: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                vehicleId: Guid.Empty,
                startDate: DateTime.UtcNow.AddDays(-1)));

        Assert.Contains("Vehicle Id cannot be empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsFutureStartDate()
    {
        var ex = Assert.Throws<DomainValidationException>(() =>
            Rental.Create(
                id: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                vehicleId: Guid.NewGuid(),
                startDate: DateTime.UtcNow.AddDays(1)));

        Assert.Contains("Cannot start a rental in the future", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsEndDateBeforeStartDate()
    {
        var startDate = DateTime.UtcNow.AddDays(-2);

        var ex = Assert.Throws<DomainValidationException>(() =>
            Rental.Create(
                id: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                vehicleId: Guid.NewGuid(),
                startDate: startDate,
                endDate: startDate.AddDays(-1)));

        Assert.Contains("End Date cannot be before Start Date", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithValidData_ReturnsRental()
    {
        var id = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-3);

        var rental = Rental.Create(id, customerId, vehicleId, startDate);

        Assert.Equal(id, rental.Id);
        Assert.Equal(customerId, rental.CustomerId);
        Assert.Equal(vehicleId, rental.VehicleId);
        Assert.Equal(startDate, rental.StartDate);
        Assert.Null(rental.EndDate);
        var domainEvent = Assert.Single(rental.DomainEvents);
        var rentedEvent = Assert.IsType<VehicleRentedDomainEvent>(domainEvent);
        Assert.Equal(id, rentedEvent.RentalId);
        Assert.Equal(vehicleId, rentedEvent.VehicleId);
        Assert.Equal(customerId, rentedEvent.CustomerId);
        Assert.Equal(startDate, rentedEvent.StartDate);
    }

    [Fact]
    public void Rehydrate_DoesNotEmitDomainEvents()
    {
        var rental = Rental.Rehydrate(
            id: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            startDate: DateTime.UtcNow.AddDays(-3),
            endDate: null);

        Assert.Empty(rental.DomainEvents);
    }

    [Fact]
    public void Return_WithValidData_SetsEndDateAndEmitsDomainEvent()
    {
        var startDate = DateTime.UtcNow.AddDays(-3);
        var endDate = DateTime.UtcNow.AddDays(-1);

        var rental = Rental.Rehydrate(
            id: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            startDate: startDate,
            endDate: null);

        rental.Return(endDate);

        Assert.Equal(endDate, rental.EndDate);
        var domainEvent = Assert.Single(rental.DomainEvents);
        var returnedEvent = Assert.IsType<VehicleReturnedDomainEvent>(domainEvent);
        Assert.Equal(rental.Id, returnedEvent.RentalId);
        Assert.Equal(rental.VehicleId, returnedEvent.VehicleId);
        Assert.Equal(rental.CustomerId, returnedEvent.CustomerId);
        Assert.Equal(endDate, returnedEvent.EndDate);
    }

    [Fact]
    public void Return_WhenAlreadyReturned_Throws()
    {
        var rental = Rental.Rehydrate(
            id: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            startDate: DateTime.UtcNow.AddDays(-3),
            endDate: DateTime.UtcNow.AddDays(-1));

        var ex = Assert.Throws<BusinessRuleViolationException>(() => rental.Return(DateTime.UtcNow));

        Assert.Contains("already been returned", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
