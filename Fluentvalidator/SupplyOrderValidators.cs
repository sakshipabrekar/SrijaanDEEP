using FluentValidation;
using SrijanDEEP.API.DTOs;

namespace SrijanDEEP.API.Validators;

public class CreateSupplyOrderDtoValidator : AbstractValidator<CreateSupplyOrderDto>
{
    public CreateSupplyOrderDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("A valid ProductId is required.");

        RuleFor(x => x.URN_No)
            .NotEmpty().WithMessage("URN_No is required (FK to Vendor_Master).")
            .MaximumLength(100);

        RuleFor(x => x.PO_No)
            .NotEmpty().WithMessage("PO_No (Purchase Order Number) is required.")
            .MaximumLength(100);

        RuleFor(x => x.PRO_No)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.PRO_No));

        RuleFor(x => x.Qty_Ordered)
            .GreaterThan(0).WithMessage("Qty_Ordered must be greater than zero.");

        RuleFor(x => x.Unit_Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Unit_Rate cannot be negative.");

        RuleFor(x => x.Qty_Supplied)
            .GreaterThanOrEqualTo(0).WithMessage("Qty_Supplied cannot be negative.")
            .LessThanOrEqualTo(x => x.Qty_Ordered)
            .WithMessage("Qty_Supplied cannot exceed Qty_Ordered.");
    }
}

public class UpdateSupplyOrderDtoValidator : AbstractValidator<UpdateSupplyOrderDto>
{
    public UpdateSupplyOrderDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("A valid ProductId is required.");

        RuleFor(x => x.URN_No)
            .NotEmpty().WithMessage("URN_No is required (FK to Vendor_Master).")
            .MaximumLength(100);

        RuleFor(x => x.PO_No)
            .NotEmpty().WithMessage("PO_No (Purchase Order Number) is required.")
            .MaximumLength(100);

        RuleFor(x => x.PRO_No)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.PRO_No));

        RuleFor(x => x.Qty_Ordered)
            .GreaterThan(0).WithMessage("Qty_Ordered must be greater than zero.");

        RuleFor(x => x.Unit_Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Unit_Rate cannot be negative.");

        RuleFor(x => x.Qty_Supplied)
            .GreaterThanOrEqualTo(0).WithMessage("Qty_Supplied cannot be negative.")
            .LessThanOrEqualTo(x => x.Qty_Ordered)
            .WithMessage("Qty_Supplied cannot exceed Qty_Ordered.");
    }
}