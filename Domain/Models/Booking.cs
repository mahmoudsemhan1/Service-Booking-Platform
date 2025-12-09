using Domain.Models.Base;
using Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Booking:AuditableEntity
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service? Service { get; set; }


        public string UserId { get; set; } = null!;
        public User? User { get; set; }


        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }


        public DateTime BookingDate { get; set; }
        public TimeSpan? BookingTime { get; set; }


        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public decimal TotalPrice { get; set; }


        public int? PaymentId { get; set; }
        public Payment? Payment { get; set; }
    }
}
