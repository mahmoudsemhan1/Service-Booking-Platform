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
        public int ServiceId { get; private set; }
        public int ProviderId { get; private set; }
        public string UserId { get; private set; } = null!;//FK 
        public BookingStatus Status { get; private set; }=BookingStatus.Pending;
        private Booking() { } // EF Core

        public Booking(string userId, int serviceId, int providerId)
        {
            UserId = userId;
            ServiceId = serviceId;
            ProviderId = providerId;
            Status = BookingStatus.Pending;
        }
        public DateTime BookingDate { get; set; }
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
