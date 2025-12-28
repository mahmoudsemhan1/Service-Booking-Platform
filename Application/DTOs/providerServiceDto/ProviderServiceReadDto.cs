using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.providerServiceDto
{
    public class ProviderServiceReadDto
    {
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; } 
        public string? PrimaryImageUrl { get; set; }
    }
}
