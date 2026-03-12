using System.ComponentModel.DataAnnotations;

namespace RentingPrototype.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute()
    {
        ErrorMessage = "Guid value cannot be empty.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is Guid guid && guid != Guid.Empty)
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }
}
