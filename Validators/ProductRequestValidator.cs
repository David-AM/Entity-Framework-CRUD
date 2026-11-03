using EF_CRUD.Dtos;
using FluentValidation;

namespace EF_CRUD.Validators;

public sealed class ProductRequestValidator : AbstractValidator<ProductRequestDto>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(120);

        RuleFor(x => x.Price)
            .InclusiveBetween(0m, 1_000_000_000m).WithMessage("Price must be between 0 and 1,000,000,000.")
            .PrecisionScale(18, 2, false).WithMessage("Price can have at most 2 decimal places.");

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Choose a category.");

        RuleFor(x => x.SupplierId)
            .GreaterThan(0).WithMessage("Choose a supplier.");
    }
}
