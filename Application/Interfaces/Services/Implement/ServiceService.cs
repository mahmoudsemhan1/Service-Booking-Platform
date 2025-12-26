using Application.DTOs.Service;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.IServices;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Implement
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ServiceService(IUnitofWork unitofWork, IMapper mapper, IFileService fileService)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ServiceReadDto>> GetAllAsync()
        {
            var services= await _unitofWork.Services.GetAllAsync();
      
            return   _mapper.Map<IEnumerable<ServiceReadDto>>(services);
        }

        public async Task<ServiceReadDto?> GetByIdAsync(int id)
        {
            var service=await _unitofWork.Services.GetByIdAsync(id);
            return service == null ? null : _mapper.Map<ServiceReadDto>(service);
        }


        public async Task<ServiceReadDto> CreateAsync(ServiceCreateDto dto)
        {
            var service = _mapper.Map<Service>(dto);

            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                for (int i = 0; i < dto.ImageFiles.Count; i++)
                {
                    // رفع الملف الحالي
                    var path = await _fileService.UploadFileAsync(dto.ImageFiles[i], "services");

                    // التأكد من وجود قيمة في قائمة الـ Boolean المقابلة، وإلا نعتبرها false
                    bool isPrimary = (dto.IsPrimaryStatus != null && dto.IsPrimaryStatus.Count > i)
                                     ? dto.IsPrimaryStatus[i]
                                     : false;

                    service.AddImage(path, isPrimary);
                }
            }
            await _unitofWork.Services.AddAsync(service);
            await _unitofWork.CompleteAsync();

            return  _mapper.Map<ServiceReadDto>(service);
        }
        public async Task<ServiceReadDto> UpdateAsync(int id, ServiceUpdateDto dto)
        {
            var service = await _unitofWork.Services.GetByIdAsync(id);
            if (service == null)
                throw new KeyNotFoundException("Service not found");

            service.Update(
                dto.Title,
                dto.Price,
                dto.DurationMinutes,
                dto.Description
            );

            await _unitofWork.CompleteAsync();

            return _mapper.Map<ServiceReadDto>(service);

        }
        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _unitofWork.Services.GetByIdAsync(id);
            if (service == null) throw new KeyNotFoundException("Service not found");

            await _unitofWork.Services.DeleteAsync(service);
            await _unitofWork.CompleteAsync();
            return true;

        }

       
    }
}
