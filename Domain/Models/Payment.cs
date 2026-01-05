using Domain.Models.Base;
using Domain.Models.Enum;

namespace Domain.Models
{
    public class Payment : AuditableEntity
    {
        public int Id { get; private set; }


        public int? BookingId { get; private set; }
        public string UserId { get; private set; } = null!;


        public decimal Amount { get; private set; }
        public PaymentMethod Method { get; private set; } = PaymentMethod.Unknown;
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public string? TransactionId { get; private set; }
        public string? RawResponse { get; private set; }
        public Booking? Booking { get; set; }


        private Payment() { } //for EF -> need constructor فاضي 
        public Payment(
           int bookingId,
        string userId,
        decimal amount,
        PaymentMethod method = PaymentMethod.Unknown)
        {
            BookingId = bookingId;
            UserId = userId;
            Amount = amount;
          Status = PaymentStatus.Pending;
            method = Method;
        }


        public void MarkAsSuccess(string transactionId , string? rawResponse=null)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("only pending payment can be marked as a success");

            Status = PaymentStatus.Success;
            TransactionId = transactionId;
            RawResponse = rawResponse;
        }

        public void MarkAsFailed(string? rawResponse = null)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("only pending payment can be marked as a failed");

            Status = PaymentStatus.Failed;
            RawResponse = rawResponse;
        }

        public void Refund()
        {
            if (Status != PaymentStatus.Success)
                throw new InvalidOperationException("Only successful payments can be refunded.");

            Status = PaymentStatus.Refunded;
        }
       


    }
}