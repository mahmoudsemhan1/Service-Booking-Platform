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
        public int ProviderId { get; set; }
        public string UserId { get; set; } = null!;//FK 
        public DateTime BookingDate { get; set; }

        public BookingStatus Status { get; private set; }
        // Domain logic: Confirm
        public void Confirm()
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidOperationException("Booking cannot be confirmed.");

            Status = BookingStatus.Confirmed;
        }
        public void Cancel()
        {
            if (Status == BookingStatus.Completed)
                throw new InvalidOperationException("Completed booking cannot be cancelled.");

            Status = BookingStatus.Cancelled;
        }

        // Domain logic: Complete
        public void Complete()
        {
            if (Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be completed.");

            Status = BookingStatus.Completed;
        }
        public decimal TotalPrice { get; set; }
        public int? PaymentId { get; set; }
        public TimeSpan? BookingTime { get; set; }


        public Service? Service { get; set; }
        public Provider? Provider { get; set; }
        public Payment? Payment { get; set; }
    }
}
