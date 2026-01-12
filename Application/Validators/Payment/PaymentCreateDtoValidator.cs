
using Application.DTOs.Payment;
using FluentValidation;

namespace Application.Validators.Payment
{
    public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
    {
        public PaymentCreateDtoValidator()
        {
            // Validation rules for PaymentCreateDto
            RuleFor(x => x.BookingId)
                .GreaterThan(0).WithMessage("BookingId must be greater than 0.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Method)
                .IsInEnum().WithMessage("Invalid payment method.");
        }
    }
}
