using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? PhotoPath { get; set; }

        public UserProfile? Profile { get; set; }
        public Provider? Provider { get; set; }


        // Related collections
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Payment>? Payments { get; set; }




    }
}
