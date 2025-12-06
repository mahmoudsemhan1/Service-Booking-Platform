using Domain.Models.Base;
using Domain.Models.Enum;

namespace Domain.Models
{
    public class Payment : AuditableEntity
    {
        public int Id { get; set; }


        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }


        public string UserId { get; set; } = null!;
        public User? User { get; set; }


        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Unknown;
        public string? TransactionId { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? RawResponse { get; set; }

    }
}