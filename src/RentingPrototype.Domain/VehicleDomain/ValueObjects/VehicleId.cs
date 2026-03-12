using RentingPrototype.Domain.Common.Exceptions;

namespace RentingPrototype.Domain.VehicleDomain.ValueObjects;

public readonly record struct VehicleId(Guid Value)
{
    public static VehicleId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("D");

    public static VehicleId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainValidationException("VehicleId cannot be empty.");

        return new VehicleId(value);
    }
}
