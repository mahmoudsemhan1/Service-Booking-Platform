using Domain.Constants;
using FluentValidation;
using static Application.DTOs.Account.Account;

namespace Application.Validators.Account
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {

        public RegisterDtoValidator()
        {
            //Fullname validation
            RuleFor(x=>x.FullName)
                .NotNull().WithMessage("FullName is required")
                .Length(3, 50).WithMessage("FullName must be between 3 and 50 characters")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("FullName can only contain letters and spaces");
            //Email validation
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
            //Password validation
            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            //Role validation
            RuleFor(x => x.Role)
                .NotNull().WithMessage("Role is required")
                .Must(role => new List<string> { AppRoles.User, AppRoles.Provider }
                .Contains(role, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Role must be either 'User' or 'Provider'");
            //Bio validation
            RuleFor(x => x.bio)
                .MaximumLength(250).WithMessage("Bio cannot exceed 250 characters");



        }
    }
}
