using Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Payment
{
    public class PaymentCreateDto
    {
        public int BookingId { get; set; }

        public string UserId { get; set; } = null!;

        
        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }
    }
}
