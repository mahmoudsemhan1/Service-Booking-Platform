using Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Booking
{
    public class BookingUpdateDto
    {
        public DateTime? BookingDate { get; set; }
        public TimeSpan? BookingTime { get; set; }
        public BookingStatus? Status { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
