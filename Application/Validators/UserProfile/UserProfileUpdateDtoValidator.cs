using FluentValidation;
using Application.DTOs.UserProfile;

namespace Application.Validators.UserProfile
{
    public class UserProfileUpdateDtoValidator :AbstractValidator<UpdateProfileDto>
    {
        public UserProfileUpdateDtoValidator()
        {
            RuleFor(x => x.Bio)
                .MaximumLength(250).WithMessage("Bio cannot exceed 250 characters.");
            RuleFor(x => x.ProfileImage)
                .Must(file => file == null || file.Length <= 3 * 1024 * 1024) // 5 MB limit
                .WithMessage("Profile image size cannot exceed 5 MB.")
                .Must(file => file == null || 
                              file.ContentType == "image/jpeg" || 
                              file.ContentType == "image/png" || 
                              file.ContentType == "image/gif")
                .WithMessage("Profile image must be a JPEG, PNG, or GIF file.");
        }
    }
}
