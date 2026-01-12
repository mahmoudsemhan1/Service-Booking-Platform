using Application.DTOs.Booking;
using FluentValidation;

namespace Application.Validators.Booking
{
    public class BookingCreateDtoValidator : AbstractValidator<BookingCreateDto>
    {
        public BookingCreateDtoValidator()
        {
            // Validation rules for BookingCreateDto
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("ServiceId must be greater than 0.");

            RuleFor(x => x.ProviderId)
                .GreaterThan(0).WithMessage("ProviderId must be greater than 0.");

            RuleFor(x => x.BookingDate)
                .GreaterThan(DateTime.UtcNow.Date).WithMessage("BookingDate must be in the future.");

            RuleFor(x => x.BookingTime)
                .NotNull().WithMessage("BookingTime is required.");



        }
    }
}
