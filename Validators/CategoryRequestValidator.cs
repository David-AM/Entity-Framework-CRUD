using EF_CRUD.Dtos;
using FluentValidation;

namespace EF_CRUD.Validators;

public sealed class CategoryRequestValidator : AbstractValidator<CategoryRequestDto>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(80);
    }
}
