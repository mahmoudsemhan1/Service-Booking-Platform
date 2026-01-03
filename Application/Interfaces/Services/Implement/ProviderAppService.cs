using Application.DTOs.providerServiceDto;
using Application.Interfaces.Services.IProviderSerivce;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Implement
{
    public class ProviderAppService : IProviderAppService
    {
        public readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;
        public ProviderAppService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;
        }
        public Task AssignServicesAsync(string userId, AssignServicesDto dto)
        {
            var userProvider = _unitofWork.Providers
                .GetByUserIdWithDetailsAsync(userId).Result;
            if (userProvider == null)
            {
                throw new Exception("Provider not found");
            }

            foreach (var serviceItem in dto.Services)
            {
                var existingService = userProvider.ProviderServices
                    .FirstOrDefault(ps => ps.ServiceId == serviceItem.ServiceId);
                if (existingService != null)
                {
                    // Update pricing
                    userProvider.UpdateServicePricing(
                        serviceItem.ServiceId,
                        serviceItem.Price,
                        serviceItem.DiscountedPrice);
                }
                else
                {
                    // Add new service
                    userProvider.AddService(
                        serviceItem.ServiceId,
                        serviceItem.Price,
                        serviceItem.DiscountedPrice);
                }
            }
            return _unitofWork.CompleteAsync();
        }

        public async Task<ProviderProfileReadDto> GetProfileAsync(string userId)
        {
            var userProvider  =await _unitofWork.Providers
                .GetByUserIdWithDetailsAsync(userId);
            if (userProvider == null)
            {
                throw new Exception("Provider not found");
            }
            var providerProfileDto = _mapper.Map<ProviderProfileReadDto>(userProvider);
            return (providerProfileDto);
        }

        public async Task RemoveServiceAsync(string userId, int serviceId)
        {
            var provider = await _unitofWork.Providers.GetByUserIdWithDetailsAsync(userId);

            if (provider == null)
            {
                throw new Exception("Provider not found");
            }

            provider.RemoveService(serviceId);

            await _unitofWork.CompleteAsync();
        }

        public async Task ToggleProviderStatusAsync(string userId)
        {
          var provider= await _unitofWork.Providers.GetByUserIdWithDetailsAsync(userId);
            if (provider == null)
            {
                throw new Exception("Provider not found");
            }
            provider.ToggleStatus();
            await _unitofWork.CompleteAsync();
        }

        public async Task UpdateBusinessInfoAsync(string userId, UpdateBusinessInfoDto dto)
        {
            var provider= await _unitofWork.Providers.GetByUserIdWithDetailsAsync(userId);
            if (provider == null)
            {
                throw new Exception("Provider not found");
            }
            TimeSpan? open = !string.IsNullOrEmpty(dto.OpenTime) ? TimeSpan.Parse(dto.OpenTime) : null;
            TimeSpan? close = !string.IsNullOrEmpty(dto.CloseTime) ? TimeSpan.Parse(dto.CloseTime) : null;

            provider.UpdateBusinessInfo(dto.BusinessName, dto.Address, open, close);
            await _unitofWork.CompleteAsync();
        }
    }
}
