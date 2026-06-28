using FluentValidation;
using SrijanDEEP.API.DTOs;

namespace SrijanDEEP.API.Validators;

public class CreateVendorDtoValidator : AbstractValidator<CreateVendorDto>
{
    public CreateVendorDtoValidator()
    {
        RuleFor(x => x.URN_No)
            .NotEmpty().WithMessage("URN_No is required (it is the primary key).")
            .MaximumLength(100).WithMessage("URN_No cannot exceed 100 characters.");

        RuleFor(x => x.Vendor_Org_Name)
            .NotEmpty().WithMessage("Vendor organisation name is required.")
            .MaximumLength(250).WithMessage("Vendor organisation name cannot exceed 250 characters.");

        RuleFor(x => x.Vendor_Org_Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Vendor_Org_Email))
            .WithMessage("Vendor organisation email must be a valid email address.");

        RuleFor(x => x.Nodal_Officer_Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Nodal_Officer_Email))
            .WithMessage("Nodal officer email must be a valid email address.");

        RuleFor(x => x.Nodal_Officer_Mobile)
            .Matches(@"^[0-9]{10}$").When(x => !string.IsNullOrEmpty(x.Nodal_Officer_Mobile))
            .WithMessage("Nodal officer mobile must be a 10-digit number.");

        RuleFor(x => x.PAN_No)
            .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").When(x => !string.IsNullOrEmpty(x.PAN_No))
            .WithMessage("PAN_No format is invalid. Expected: AAAAA9999A");

        RuleFor(x => x.GST_No)
            .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
            .When(x => !string.IsNullOrEmpty(x.GST_No))
            .WithMessage("GST_No format is invalid.");

        RuleFor(x => x.CIN_No)
            .MaximumLength(21).When(x => !string.IsNullOrEmpty(x.CIN_No))
            .WithMessage("CIN_No cannot exceed 21 characters.");
    }
}

public class UpdateVendorDtoValidator : AbstractValidator<UpdateVendorDto>
{
    public UpdateVendorDtoValidator()
    {
        RuleFor(x => x.Vendor_Org_Name)
            .NotEmpty().WithMessage("Vendor organisation name is required.")
            .MaximumLength(250).WithMessage("Vendor organisation name cannot exceed 250 characters.");

        RuleFor(x => x.Vendor_Org_Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Vendor_Org_Email))
            .WithMessage("Vendor organisation email must be a valid email address.");

        RuleFor(x => x.Nodal_Officer_Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Nodal_Officer_Email))
            .WithMessage("Nodal officer email must be a valid email address.");

        RuleFor(x => x.Nodal_Officer_Mobile)
            .Matches(@"^[0-9]{10}$").When(x => !string.IsNullOrEmpty(x.Nodal_Officer_Mobile))
            .WithMessage("Nodal officer mobile must be a 10-digit number.");

        RuleFor(x => x.PAN_No)
            .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").When(x => !string.IsNullOrEmpty(x.PAN_No))
            .WithMessage("PAN_No format is invalid. Expected: AAAAA9999A");

        RuleFor(x => x.GST_No)
            .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
            .When(x => !string.IsNullOrEmpty(x.GST_No))
            .WithMessage("GST_No format is invalid.");

        RuleFor(x => x.CIN_No)
            .MaximumLength(21).When(x => !string.IsNullOrEmpty(x.CIN_No))
            .WithMessage("CIN_No cannot exceed 21 characters.");
    }
}