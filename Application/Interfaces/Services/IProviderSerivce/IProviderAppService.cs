using Application.DTOs.providerServiceDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.IProviderSerivce
{
    public interface IProviderAppService
    {
        Task AssignServicesAsync(string userId, AssignServicesDto dto);
        Task<ProviderProfileReadDto> GetProfileAsync(string userId);

        Task UpdateBusinessInfoAsync(string userId, UpdateBusinessInfoDto dto);
        Task RemoveServiceAsync(string userId, int serviceId);
        Task ToggleProviderStatusAsync(string userId);
    }
}
