using FluentValidation;
using SrijanDEEP.API.DTOs;

namespace SrijanDEEP.API.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.UniqueReferenceNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.VendorId).GreaterThan(0).WithMessage("A valid VendorId is required.");
        RuleFor(x => x.SupplyOrderDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.SupplyOrderDate.HasValue)
            .WithMessage("Supply order date cannot be in the future.");
    }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.UniqueReferenceNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.VendorId).GreaterThan(0).WithMessage("A valid VendorId is required.");
        RuleFor(x => x.SupplyOrderDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.SupplyOrderDate.HasValue)
            .WithMessage("Supply order date cannot be in the future.");
    }
}