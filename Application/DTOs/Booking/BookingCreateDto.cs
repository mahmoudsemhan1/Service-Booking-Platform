using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Booking
{
    public class BookingCreateDto
    {
        [Required]
        public int ServiceId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int ProviderId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public TimeSpan? BookingTime { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
    }
}
