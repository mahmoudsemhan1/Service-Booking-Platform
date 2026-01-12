using Application.DTOs.Account;
using FluentValidation;

namespace Application.Validators.Account
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {

        public ChangePasswordDtoValidator()
        {
            // Validate Current Password
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");
            // Validate New Password
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters long.");
        }
    }
}
