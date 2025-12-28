using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.providerServiceDto
{
    public class UpdateBusinessInfoDto
    {
        public string BusinessName { get; set; } = null!;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? OpenTime { get; set; } 
        public string? CloseTime { get; set; }
    }
}
