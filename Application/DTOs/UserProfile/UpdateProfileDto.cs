using Microsoft.AspNetCore.Http;

namespace Application.DTOs.UserProfile
{
    public class UpdateProfileDto
    {
        public string? Bio { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
