using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Image
{
    public class ImageReadDto
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } 
        public bool IsPrimary { get; set; }
    }
}
