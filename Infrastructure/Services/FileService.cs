using Application.DTOs.UserProfile;
using Application.Interfaces.Services.IfileService;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long _maxFileSize = 2 * 1024 * 1024;
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;

        public FileService(IWebHostEnvironment env, IUnitofWork unitofWork, IMapper mapper)
        {
            _env = env;
            _unitofWork = unitofWork;
            _mapper = mapper;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, .png, and .webp are allowed.");

            if (file.Length > _maxFileSize)
                throw new ArgumentException("File size exceeds the 2MB limit.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"{folderName}/{uniqueFileName}";
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            var cleanRelativePath = relativePath.TrimStart('/', '\\');

            var fullPath = Path.Combine(_env.WebRootPath, cleanRelativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

    }
}
