using System.ComponentModel.DataAnnotations;

namespace RentingPrototype.Api.Validation;

public static class EndpointRequestValidator
{
    public static IDictionary<string, string[]>? Validate<T>(T model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model!);

        var isValid = Validator.TryValidateObject(model!, context, results, validateAllProperties: true);
        if (isValid)
            return null;

        return results
            .SelectMany(result =>
            {
                var members = result.MemberNames.Any()
                    ? result.MemberNames
                    : [string.Empty];

                return members.Select(member => new
                {
                    Member = member,
                    Error = result.ErrorMessage ?? "Invalid value."
                });
            })
            .GroupBy(item => item.Member)
            .ToDictionary(
                group => group.Key,
                group => group.Select(item => item.Error).Distinct().ToArray());
    }
}
