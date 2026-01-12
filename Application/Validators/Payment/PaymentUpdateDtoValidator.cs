using Application.DTOs.Payment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Payment
{
    public class PaymentUpdateDtoValidator :AbstractValidator<PaymentUpdateStatusDto>
    {
        public PaymentUpdateDtoValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");

            RuleFor(x => x.RawResponse)
                .MaximumLength(4000).WithMessage("RawResponse cannot exceed 4000 characters.");
        }
    }
}
