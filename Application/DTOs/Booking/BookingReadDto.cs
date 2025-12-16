using Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Booking
{
    public class BookingReadDto
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;

        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;

        public DateTime BookingDate { get; set; }
        public TimeSpan? BookingTime { get; set; }

        public BookingStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
