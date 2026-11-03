using FluentValidation.Results;

namespace EF_CRUD.Common;

public static class ValidationResultExtensions
{
    /// <summary>Groups FluentValidation failures by property name: { "Name": ["...", "..."] }.</summary>
    public static IReadOnlyDictionary<string, string[]> ToErrorDictionary(this ValidationResult result) =>
        result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).Distinct().ToArray());
}
