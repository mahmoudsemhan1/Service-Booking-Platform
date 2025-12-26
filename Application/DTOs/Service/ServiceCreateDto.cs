using Application.DTOs.Image;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Service
{
    public class ServiceCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }
        // الملفات الفعلية
        public List<IFormFile> ImageFiles { get; set; } = new();

        // حالات الـ Primary لكل ملف بالترتيب
        public List<bool> IsPrimaryStatus { get; set; } = new();
    }
}
