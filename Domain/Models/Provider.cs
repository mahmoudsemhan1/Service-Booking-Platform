using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Provider: AuditableEntity
    {
        public int Id { get; set; }

        // FK to Domain User
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public string BusinessName { get; set; } = null!;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        // Navigation properties
        public ICollection<ProviderService>? ProviderServices { get; set; }
        public ICollection<Review>? Reviews { get; set; }

       

    }
}
