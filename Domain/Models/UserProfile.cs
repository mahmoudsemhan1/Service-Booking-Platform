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
        public int  Id { get; set; } 

        public string UserId { get; set; } = null!; //FK =>Applicationuser 
        public string? Bio { get; set; }
        public string? PhotoPath { get; set; }

        

    }
}
