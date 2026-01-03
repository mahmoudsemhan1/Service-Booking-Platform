using Application.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.IServices
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceReadDto>> GetAllAsync();
        Task<ServiceReadDto?> GetByIdAsync(int id);
        Task<ServiceReadDto> CreateAsync(ServiceCreateDto dto ,string userId);
        Task<ServiceReadDto> UpdateAsync(int id, ServiceUpdateDto dto,string userId);
        Task<bool> DeleteAsync(int id,string userId);

    }
}
