using Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Payment
{
    public class PaymentReadDto
    {
        public int Id { get; set; }

        public int? BookingId { get; set; }
        public string UserId { get; set; } = null!;

        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
