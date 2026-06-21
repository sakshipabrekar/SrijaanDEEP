
using FluentValidation;
using SrijanDEEP.API.DTOs;

namespace SrijanDEEP.API.Validators;

public class CreateVendorDtoValidator : AbstractValidator<CreateVendorDto>
{
    public CreateVendorDtoValidator()
    {
        RuleFor(x => x.VendorOrganisationName)
            .NotEmpty().WithMessage("Vendor organisation name is required.")
            .MaximumLength(250);

        RuleFor(x => x.VendorOrganisationEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.VendorOrganisationEmail))
            .WithMessage("Vendor organisation email must be a valid email address.");

        RuleFor(x => x.NodalOfficerEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.NodalOfficerEmail))
            .WithMessage("Nodal officer email must be a valid email address.");

        RuleFor(x => x.NodalOfficerMobile)
            .Matches(@"^[0-9]{10}$").When(x => !string.IsNullOrEmpty(x.NodalOfficerMobile))
            .WithMessage("Nodal officer mobile must be a 10-digit number.");

        RuleFor(x => x.PANNumber)
            .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").When(x => !string.IsNullOrEmpty(x.PANNumber))
            .WithMessage("PAN number format is invalid.");

        RuleFor(x => x.GSTNumber)
            .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
            .When(x => !string.IsNullOrEmpty(x.GSTNumber))
            .WithMessage("GST number format is invalid.");
    }
}

public class UpdateVendorDtoValidator : AbstractValidator<UpdateVendorDto>
{
    public UpdateVendorDtoValidator()
    {
        RuleFor(x => x.VendorOrganisationName)
            .NotEmpty().WithMessage("Vendor organisation name is required.")
            .MaximumLength(250);

        RuleFor(x => x.VendorOrganisationEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.VendorOrganisationEmail))
            .WithMessage("Vendor organisation email must be a valid email address.");

        RuleFor(x => x.NodalOfficerEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.NodalOfficerEmail))
            .WithMessage("Nodal officer email must be a valid email address.");

        RuleFor(x => x.NodalOfficerMobile)
            .Matches(@"^[0-9]{10}$").When(x => !string.IsNullOrEmpty(x.NodalOfficerMobile))
            .WithMessage("Nodal officer mobile must be a 10-digit number.");

        RuleFor(x => x.PANNumber)
            .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").When(x => !string.IsNullOrEmpty(x.PANNumber))
            .WithMessage("PAN number format is invalid.");

        RuleFor(x => x.GSTNumber)
            .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
            .When(x => !string.IsNullOrEmpty(x.GSTNumber))
            .WithMessage("GST number format is invalid.");
    }
}
