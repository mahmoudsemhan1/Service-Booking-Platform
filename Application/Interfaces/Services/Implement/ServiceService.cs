using Application.DTOs.Service;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.IServices;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
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
            var services = await _unitofWork.Services.GetAllAsync();

            return _mapper.Map<IEnumerable<ServiceReadDto>>(services);
        }

        public async Task<ServiceReadDto?> GetByIdAsync(int id)
        {
            var service = await _unitofWork.Services.GetByIdAsync(id);
            return service == null ? null : _mapper.Map<ServiceReadDto>(service);
        }


        public async Task<ServiceReadDto> CreateAsync(ServiceCreateDto dto, string userId)
        {
            var provider = (await _unitofWork.Providers.FindAsync(p => p.UserId == userId)).FirstOrDefault();
            if (provider == null) throw new Exception("Provider not found for the given user.");

            await _unitofWork.BeginTransactionAsync();

            try
            {
                var service = new Service(dto.Title, dto.Price, dto.Description, dto.DurationMinutes);

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

                provider.AddService(service.Id, dto.Price);

                await _unitofWork.CompleteAsync();
                await _unitofWork.CommitTransactionAsync();

                return _mapper.Map<ServiceReadDto>(service);
            }
            catch (Exception)
            {
                await _unitofWork.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<ServiceReadDto> UpdateAsync(int id, ServiceUpdateDto dto, string userId)
        {
            var service = await _unitofWork.Services.GetByIdWithImagesAsync(id);
            if (service == null) throw new Exception("Service not found");

            var provider = (await _unitofWork.Providers.FindAsync(p => p.UserId == userId)).FirstOrDefault();
            if (provider == null) throw new UnauthorizedAccessException("Provider not authorized.");
            provider.UpdateServicePricing(id, dto.Price, dto.DiscountedPrice);
            service.Update(dto.Title, dto.Price, dto.DurationMinutes, dto.Description);
            // delete the old images 
            if (dto.ImageIdsToDelete != null)
            {
                foreach (var imageId in dto.ImageIdsToDelete)
                {
                    var image = service.Images.FirstOrDefault(i => i.Id == imageId);
                    if (image != null)
                    {
                        // مسح الملف من الهارد ديسك أولاً
                        _fileService.DeleteFile(image.ImagePath);
                        // مسح السجل من الداتابيز
                        service.RemoveImage(imageId);
                    }
                }
            }
            if (dto.NewImageFiles != null)
            {
                for (int i = 0; i < dto.NewImageFiles.Count; i++)
                {
                    var path = await _fileService.UploadFileAsync(dto.NewImageFiles[i], "services");
                    bool isPrimary = dto.NewIsPrimaryStatus != null && dto.NewIsPrimaryStatus.Count > i
                                     ? dto.NewIsPrimaryStatus[i] : false;

                    service.AddImage(path, isPrimary);
                }
            }

            await _unitofWork.CompleteAsync();

            return _mapper.Map<ServiceReadDto>(service);

        }
        public async Task<bool> DeleteAsync(int id,string userId)
        {
            var provider = (await _unitofWork.Providers.FindAsync(p => p.UserId == userId)).FirstOrDefault();
            if (provider == null) throw new UnauthorizedAccessException("Provider not found.");

            var service = await _unitofWork.Services.GetByIdWithImagesAsync(id);

            if (service == null) throw new KeyNotFoundException("Service not found");
            if (service == null) throw new KeyNotFoundException("Service not found");

            provider.RemoveService(id);

            //are there other providers using this service 
            // if no other providers use it , delete it from the services table
            // check in the provider services table
            // get all provider services with this service id
            // if there are no other providers using it , delete the service and its images from the file system
            // then delete the service record 
            
            var otherProviders = await _unitofWork.ProviderServices.FindAsync(ps => ps.ServiceId == id);
            if (!otherProviders.Any())
            {
                foreach (var image in service.Images)
                {
                    _fileService.DeleteFile(image.ImagePath);
                }

                await _unitofWork.Services.DeleteAsync(service);
            }

            await _unitofWork.CompleteAsync();
            return true;

        }


    }
}
