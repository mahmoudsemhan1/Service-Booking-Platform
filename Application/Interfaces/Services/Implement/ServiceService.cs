using Application.Common.page;
using Application.DTOs.Paged;
using Application.DTOs.Service;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.IServices;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using System.Linq.Expressions;
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

            // this for tracking the uploaded files in case of rollback
            var uploadefiles = new List<string>();
            await _unitofWork.BeginTransactionAsync();

            try
            {
                var service = new Service(dto.Title, dto.Price, dto.Description, dto.DurationMinutes);

                if (dto.ImageFiles != null && dto.ImageFiles.Any())
                {
                    for (int i = 0; i < dto.ImageFiles.Count; i++)
                    {
                        // apload each file and get the path , if any error occurs rollback the transaction and delete the uploaded files
                        var path = await _fileService.UploadFileAsync(dto.ImageFiles[i], "services");

                        uploadefiles.Add(Path.Combine("services", path));


                        // التأكد من وجود قيمة في قائمة الـ Boolean المقابلة، وإلا نعتبرها false
                        bool isPrimary = (dto.IsPrimaryStatus != null && dto.IsPrimaryStatus.Count > i)
                                         ? dto.IsPrimaryStatus[i]
                                         : false;

                        service.AddImage(path, isPrimary);
                    }
                }
                await _unitofWork.Services.AddAsync(service);
                //the first save to generate the service id
                await _unitofWork.CompleteAsync();

                //then add the service to the provider services with the price
                provider.AddService(service.Id, dto.Price);

                await _unitofWork.CompleteAsync();
                await _unitofWork.CommitTransactionAsync();

                return _mapper.Map<ServiceReadDto>(service);
            }
            catch (Exception)
            {
                await _unitofWork.RollbackTransactionAsync();
                // delete the uploaded files in case of error
                foreach (var filePath in uploadefiles)
                {
                    _fileService.DeleteFile(filePath);
                }
                throw;
            }
        }
        public async Task<ServiceReadDto> UpdateAsync(int id, ServiceUpdateDto dto, string userId)
        {
            var service = await _unitofWork.Services.GetByIdWithImagesAsync(id);
            if (service == null) throw new Exception("Service not found");

            var provider = (await _unitofWork.Providers.GetByUserIdWithDetailsAsync(userId));
            if (provider == null) throw new UnauthorizedAccessException("Provider not authorized.");

            //this lists to track files to delete from disk in case of rollback
            var filesToDeleteFromDisk = new List<string>();
            var newlyUploadedFiles = new List<string>();

            await _unitofWork.BeginTransactionAsync();
            try
            {
                provider.UpdateServicePricing(id, dto.Price, dto.DiscountedPrice);
                service.Update(dto.Title, dto.Price, dto.DurationMinutes, dto.Description);
                // deal with the image that will  delete 
                if (dto.ImageIdsToDelete != null)
                {
                    foreach (var imageId in dto.ImageIdsToDelete)
                    {
                        var image = service.Images.FirstOrDefault(i => i.Id == imageId);
                        if (image != null)
                        {
                            // add the file path to the list to delete from disk later, but dont delete it now in case of rollback 
                            filesToDeleteFromDisk.Add(Path.Combine("services", image.ImagePath));
                            service.RemoveImage(imageId);
                        }
                    }
                }
                // deal with the new uploaded images
                if (dto.NewImageFiles != null)
                {
                    for (int i = 0; i < dto.NewImageFiles.Count; i++)
                    {
                        var path = await _fileService.UploadFileAsync(dto.NewImageFiles[i], "services");
                        // track the newly uploaded files to delete in case of rollback
                        newlyUploadedFiles.Add(Path.Combine("services", path));
                        bool isPrimary = dto.NewIsPrimaryStatus != null && dto.NewIsPrimaryStatus.Count > i
                                         ? dto.NewIsPrimaryStatus[i] : false;

                        service.AddImage(path, isPrimary);
                    }
                }
                await _unitofWork.CompleteAsync();
                await _unitofWork.CommitTransactionAsync();

                // now delete the files from disk that were marked for deletion
                foreach (var filePath in filesToDeleteFromDisk)
                {
                    _fileService.DeleteFile(filePath);
                }
                return _mapper.Map<ServiceReadDto>(service);
            }




            catch (Exception)
            {
                await _unitofWork.RollbackTransactionAsync();
                // delete the newly uploaded files in case of error
                foreach (var filePath in newlyUploadedFiles)
                {
                    _fileService.DeleteFile(filePath);
                }
                throw;
            }

        }
        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var provider = await _unitofWork.Providers.GetByUserIdWithDetailsAsync(userId);
            if (provider == null) throw new UnauthorizedAccessException("Provider not found.");

            var service = await _unitofWork.Services.GetByIdWithImagesAsync(id);

            if (service == null) throw new KeyNotFoundException("Service not found");

            provider.RemoveService(id);

            //are there other providers using this service 
            // if no other providers use it , delete it from the services table
            // check in the provider services table
            // get all provider services with this service id
            // if there are no other providers using it , delete the service and its images from the file system
            // then delete the service record 

            // make sure to save the changes to provider services first , then check for other providers 
            var otherProviders = await _unitofWork.ProviderServices.FindAsync(ps => ps.ServiceId == id && ps.ProviderId !=provider.Id) ;
            if (!otherProviders.Any())
            {
                foreach (var image in service.Images)
                {
                    var pathOnDisk = Path.Combine("services", image.ImagePath);
                    _fileService.DeleteFile(pathOnDisk);
                }

                await _unitofWork.Services.DeleteAsync(service);
            }

            await _unitofWork.CompleteAsync();
            return true;

        }

        public async Task<PagedResultDto<ServiceReadDto>> GetPagedAsync(PaginationParams paging)
        {
            // if there is a search term , filter by title
            Expression<Func<Service, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(paging.Search))
            {
                var search = paging.Search.Trim().ToLower();
                filter = s => s.Title.ToLower().Contains(search) ||
                              s.Description.ToLower().Contains(search);
            }


            // from the generic repository get the item and total count
            var (items, totalCount) = await  _unitofWork.Services.GetPagedAsync(paging.PageNumber, paging.PageSize ,predicate: filter , includeProperties: "Images");

            // map the items to dto
            var dtos = _mapper.Map<IEnumerable<ServiceReadDto>>(items);

            // create the paged result dto

            return new PagedResultDto<ServiceReadDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber =paging.PageNumber ,
                PageSize = paging.PageSize
            };
        }
    }
}
