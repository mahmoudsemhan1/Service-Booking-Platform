using Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Payment
{
    public class PaymentCreateDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }
    }
}
