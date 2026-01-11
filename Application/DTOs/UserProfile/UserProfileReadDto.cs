using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserProfile
{
    public class UserProfileReadDto
    {
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; } 
        public string? FullName { get; set; } 
    }
}

