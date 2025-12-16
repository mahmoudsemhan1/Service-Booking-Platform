using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Identity
{
    public class AppUserReadDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!; 
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }  
        public string? PhotoPath { get; set; }
    }
}
