using Domain.Models.Base;
using Domain.Models.Enum;

namespace Domain.Models
{
    public class Payment : AuditableEntity
    {
        public int Id { get; set; }


        public int? BookingId { get; set; }
        public string UserId { get; set; } = null!;


        public decimal Amount { get; set; }
        public PaymentMethod Method { get; private set; } = PaymentMethod.Unknown;
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public string? TransactionId { get; private set; }
        public string? RawResponse { get; private set; }
        public Booking? Booking { get; set; }


        public void MarkAsPaid(string transactionId)
        {
            if (Status == PaymentStatus.Success) 
                throw new InvalidOperationException("Payment already completed");

            Status = PaymentStatus.Success;
            TransactionId = transactionId;
        }

        public void MarkAsFailed(string? reason = null)
        {
            if (Status == PaymentStatus.Success)
                throw new InvalidOperationException("Successful payment cannot be failed.");

            Status = PaymentStatus.Failed;
            RawResponse = reason;
        }

        public void Refund()
        {
            if (Status != PaymentStatus.Success)
                throw new InvalidOperationException("Only successful payments can be refunded.");

            Status = PaymentStatus.Refunded;
        }


    }
}