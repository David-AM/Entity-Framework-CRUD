using EF_CRUD.Dtos;
using FluentValidation;

namespace EF_CRUD.Validators;

public sealed class SupplierRequestValidator : AbstractValidator<SupplierRequestDto>
{
    public SupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);
    }
}
