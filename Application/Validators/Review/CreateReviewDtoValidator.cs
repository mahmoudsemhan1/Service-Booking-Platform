using Application.DTOs.Review;
using FluentValidation;

namespace Application.Validators.Review
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("BookingId must be a positive integer.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithMessage("Comment cannot be empty.")
                .MaximumLength(1000)
                .WithMessage("Comment cannot exceed 1000 characters.");
           
        }
    }
}
