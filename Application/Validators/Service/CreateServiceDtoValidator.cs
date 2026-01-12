using FluentValidation;
using Application.DTOs.Service;

namespace Application.Validators.Service
{
    public class CreateServiceDtoValidator : AbstractValidator<ServiceCreateDto>
    {
        public CreateServiceDtoValidator()
        {
            
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Service Title is required.")
                .MaximumLength(100).WithMessage("Service name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than zero minutes.");

            RuleFor(x => x.ImageFiles)
                .Must((dto, imageFiles) => 
                {
                    // Ensure that for each image file, there is a corresponding IsPrimaryStatus entry
                    return imageFiles.Count == dto.IsPrimaryStatus.Count;
                })
                .WithMessage("Each image file must have a corresponding IsPrimaryStatus entry.");


        }
    }
}
