using Application.DTOs.Service;
using FluentValidation;

namespace Application.Validators.Service
{
    public class UpdateServiceDtoValidator : AbstractValidator<ServiceUpdateDto>
    {

        public UpdateServiceDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Service ID must be a positive integer.");
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Service name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .GreaterThanOrEqualTo(x => x.DiscountedPrice).WithMessage("Price must be greater than or equal to Discounted Price.");
            RuleFor(x => x.DiscountedPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Discounted Price must be zero or greater.")
                .LessThanOrEqualTo(x => x.Price).WithMessage("Discounted Price must be less than or equal to Price.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).When(x => x.DurationMinutes.HasValue).WithMessage("Duration must be greater than zero minutes.");



        }
    }
}
