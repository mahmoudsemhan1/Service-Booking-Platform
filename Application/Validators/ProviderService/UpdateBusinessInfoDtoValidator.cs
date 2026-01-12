using Application.DTOs.providerServiceDto;
using FluentValidation;

namespace Application.Validators.ProviderService
{
    public class UpdateBusinessInfoDtoValidator : AbstractValidator<UpdateBusinessInfoDto>
    {
        public UpdateBusinessInfoDtoValidator()
        {

            RuleFor(x => x.BusinessName)
                .NotEmpty().WithMessage("Business name is required.")
                .MaximumLength(100).WithMessage("Business name cannot exceed 100 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees.")

                .When(x => x.Latitude.HasValue);
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees.")
                .When(x => x.Longitude.HasValue);

            RuleFor(x => x.OpenTime)
                .Matches(@"^([01]\d|2[0-3]):?([0-5]\d)$").WithMessage("Open time must be in HH:mm format.")
                .When(x => !string.IsNullOrEmpty(x.OpenTime));

            RuleFor(x => x.CloseTime)
                .Matches(@"^([01]\d|2[0-3]):?([0-5]\d)$").WithMessage("Close time must be in HH:mm format.")
                .When(x => !string.IsNullOrEmpty(x.CloseTime));
        }
    }
}
