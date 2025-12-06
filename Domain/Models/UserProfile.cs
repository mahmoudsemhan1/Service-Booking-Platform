using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserProfile: AuditableEntity
    {
        public int Id { get; set; }


        // FK to Domain User
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;


        public string? Bio { get; set; }
        public string? PhotoPath { get; set; }
 
    }
}
