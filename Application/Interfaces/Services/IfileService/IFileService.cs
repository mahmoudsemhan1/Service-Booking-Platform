using Application.DTOs.UserProfile;
using Microsoft.AspNetCore.Http;


namespace Application.Interfaces.Services.IfileService
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        void DeleteFile(string relativePath);

    }
}
