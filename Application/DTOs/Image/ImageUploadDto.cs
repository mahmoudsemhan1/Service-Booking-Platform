using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Image
{
    public class ImageUploadDto
    {
        public IFormFile File { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
