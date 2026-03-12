using System.ComponentModel.DataAnnotations;

namespace RentingPrototype.Api.Contracts.Vehicle;

public sealed record CreateVehicleRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: StringLength(20, MinimumLength = 4)]
    string LicensePlate,
    [property: Required(AllowEmptyStrings = false)]
    [property: StringLength(100)]
    string Brand,
    [property: Required(AllowEmptyStrings = false)]
    [property: StringLength(100)]
    string Model,
    DateTime ManufactureDateUtc);
