using FluentValidation;
using srijaanDEEP.DTOs;

namespace srijaanDEEP.Fluentvalidator
{
    public class DefencePlatformCreateValidator : AbstractValidator<DefencePlatformCreateDto>
    {
        public DefencePlatformCreateValidator()
        {
            RuleFor(x => x.DefencePlatform)
                .NotEmpty().WithMessage("Defence Platform name is required.")
                .MaximumLength(200).WithMessage("Defence Platform name cannot exceed 200 characters.");
        }
    }

    public class DefencePlatformUpdateValidator : AbstractValidator<DefencePlatformUpdateDto>
    {
        public DefencePlatformUpdateValidator()
        {
            RuleFor(x => x.DefencePlatform)
                .NotEmpty().WithMessage("Defence Platform name is required.")
                .MaximumLength(200).WithMessage("Defence Platform name cannot exceed 200 characters.");
        }
    }
}