using Application.DTOs.providerServiceDto;
using FluentValidation;


namespace Application.Validators.ProviderService
{
    public class ServiceAssignmentItemDtoValidator : AbstractValidator<ServiceAssignmentItemDto>
    {
        public ServiceAssignmentItemDtoValidator()
        {
            // Validate that ServiceId is greater than 0
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("ServiceId must be greater than 0.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.DiscountedPrice)
                .GreaterThan(0).When(x => x.DiscountedPrice.HasValue)
                .WithMessage("DiscountedPrice must be greater than 0 when provided.")
                .LessThan(x => x.Price).When(x => x.DiscountedPrice.HasValue)
                .WithMessage("DiscountedPrice must be less than Price when provided.");
        }
    }
}
