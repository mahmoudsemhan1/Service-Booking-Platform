using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string? PhotoPath { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public Provider? Provider { get; set; }
        public UserProfile? Profile { get; set; }
    }
}
