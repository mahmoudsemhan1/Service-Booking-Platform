using Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Review : AuditableEntity
    {
        public int Id { get; private set; }
        public int ProviderId { get; private set; }
        public int ServiceId { get; private set; }
        public int BookingId { get; private set; } // linking to booking system to ensure credibility=> المصداقيه 
        public string UserId { get; private set; } = null!;

        [Range(1, 5)]
        public int Rating { get; private set; }
        public string? Comment { get; private set; }

        public Provider? Provider { get; private set; }
        public Service? Service { get; private set; }
        public Booking? Booking { get; private set; }
        public Review() { }
        public Review(string userId, int providerId, int serviceId, int bookingId, int rating, string? comment)
        {
            UserId = userId;
            ProviderId = providerId;
            ServiceId = serviceId;
            BookingId = bookingId;
            Rating = (rating < 1 || rating > 5) ? throw new ArgumentOutOfRangeException(nameof(rating)) : rating;
            Comment = comment;
        }


    }
}
