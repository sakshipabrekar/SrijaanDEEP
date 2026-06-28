using FluentValidation;
using SrijanDEEP.API.DTOs;

namespace SrijanDEEP.API.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.URN_No)
            .NotEmpty().WithMessage("URN_No is required (FK to Vendor_Master).")
            .MaximumLength(100);

        RuleFor(x => x.Product_Name)
            .NotEmpty().WithMessage("Product_Name is required.")
            .MaximumLength(250);

        RuleFor(x => x.Part_No)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Part_No));

        RuleFor(x => x.Defence_Platform)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Defence_Platform));

        RuleFor(x => x.Product_Type)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Product_Type));

        RuleFor(x => x.SO_No)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.SO_No));

        RuleFor(x => x.SO_Date)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.SO_Date.HasValue)
            .WithMessage("SO_Date cannot be in the future.");
    }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.URN_No)
            .NotEmpty().WithMessage("URN_No is required (FK to Vendor_Master).")
            .MaximumLength(100);

        RuleFor(x => x.Product_Name)
            .NotEmpty().WithMessage("Product_Name is required.")
            .MaximumLength(250);

        RuleFor(x => x.Part_No)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Part_No));

        RuleFor(x => x.Defence_Platform)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Defence_Platform));

        RuleFor(x => x.Product_Type)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Product_Type));

        RuleFor(x => x.SO_No)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.SO_No));

        RuleFor(x => x.SO_Date)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.SO_Date.HasValue)
            .WithMessage("SO_Date cannot be in the future.");
    }
}