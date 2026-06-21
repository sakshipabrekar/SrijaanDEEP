using FluentValidation;
using SrijanDEEP.API.DTOs;

namespace SrijanDEEP.API.Validators;

public class CreateSupplyOrderDtoValidator : AbstractValidator<CreateSupplyOrderDto>
{
    public CreateSupplyOrderDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("A valid ProductId is required.");
        RuleFor(x => x.PurchaseOrderNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.QuantityOrdered).GreaterThan(0).WithMessage("QuantityOrdered must be greater than zero.");
        RuleFor(x => x.UnitRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QuantitySupplied)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.QuantityOrdered)
            .WithMessage("QuantitySupplied cannot exceed QuantityOrdered.");
    }
}

public class UpdateSupplyOrderDtoValidator : AbstractValidator<UpdateSupplyOrderDto>
{
    public UpdateSupplyOrderDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("A valid ProductId is required.");
        RuleFor(x => x.PurchaseOrderNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.QuantityOrdered).GreaterThan(0).WithMessage("QuantityOrdered must be greater than zero.");
        RuleFor(x => x.UnitRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QuantitySupplied)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.QuantityOrdered)
            .WithMessage("QuantitySupplied cannot exceed QuantityOrdered.");
    }
}