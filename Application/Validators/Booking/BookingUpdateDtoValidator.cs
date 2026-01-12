using Application.DTOs.Booking;
using FluentValidation;

namespace Application.Validators.Booking
{
    public class BookingUpdateDtoValidator : AbstractValidator<BookingUpdateDto>
    {
        public BookingUpdateDtoValidator()
        {
            // Validation rules for BookingUpdateDto
           RuleFor(r=>r.BookingTime)
                .GreaterThan(TimeSpan.Zero).WithMessage("BookingTime must be greater than 00:00:00.");
            RuleFor(r=>r.BookingDate)
                .GreaterThan(DateTime.UtcNow.Date).WithMessage("BookingDate must be in the future.");
        }
    }
}
