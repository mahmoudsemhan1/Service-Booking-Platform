using Application.DTOs.Booking;
using Application.DTOs.Paged;
using Application.DTOs.providerServiceDto;


namespace Application.Interfaces.Services.IProviderSerivce
{
    public interface IProviderAppService
    {
        Task AssignServicesAsync(string userId, AssignServicesDto dto);
        Task<ProviderProfileReadDto> GetProfileAsync(string userId);

        Task UpdateBusinessInfoAsync(string userId, UpdateBusinessInfoDto dto);
        Task RemoveServiceAsync(string userId, int serviceId);
        Task ToggleProviderStatusAsync(string userId);

        //
    }
}
