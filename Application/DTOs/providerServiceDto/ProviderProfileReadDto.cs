using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.providerServiceDto
{
    public class ProviderProfileReadDto
    {
        public int Id { get; set; }
        public string BusinessName { get; set; } = null!;
        public string? Address { get; set; }
        public List<ProviderServiceReadDto> AssignedServices { get; set; } = new();
    }
}
