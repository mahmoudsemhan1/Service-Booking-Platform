using Application.Interfaces.Services.IfileService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting; 

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long _maxFileSize = 2 * 1024 * 1024;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if(file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty.");  

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, .png, and .webp are allowed.");

            if (file.Length > _maxFileSize) 
                throw new ArgumentException("File size exceeds the 2MB limit.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return Task.FromResult(uniqueFileName);
        }
    }
}
