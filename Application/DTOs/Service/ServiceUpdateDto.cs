using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Service
{
    public class ServiceUpdateDto
    {
        public int Id { get; set; } 
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? DurationMinutes { get; set; }
        //for adding new images
        public List<IFormFile>? NewImageFiles { get; set; }
        public List<bool>? NewIsPrimaryStatus { get; set; }

        //for deleting existing images by their IDs
        public List<int>? ImageIdsToDelete { get; set; }

    }
}
