using Application.DTOs.UserProfile;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.IUserIdentityServices;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;

namespace Application.Interfaces.Services.Implement
{
    public class UserProfilesService : Application.Interfaces.Services.IUserProfileService.IUserProfileService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        public UserProfilesService(IUnitofWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }
        public async Task<UserProfileReadDto> GetByUserIdAsync(string userId)
        {
            var profile=await  _unitOfWork.UserProfiles.GetByUserIdAsync(userId);

            var fullname = await _unitOfWork.UserProfiles.GetFullNameAsync(userId);

            var dto= _mapper.Map<UserProfileReadDto>(profile);
            dto.FullName = fullname;

            return dto;

        }

        public async Task<UserProfileReadDto> UpdateAsync(string userId, UpdateProfileDto dto)
        {
            var profile =await _unitOfWork.UserProfiles.GetByUserIdAsync(userId);
            if(profile==null)
            {
                throw new Exception("Profile not found");
            }

            if (dto.ProfileImage != null)
            {
                var newPathWithFolder= await  _fileService.UploadFileAsync(dto.ProfileImage, "profiles");

                var oldPath= profile.UpdatePhoto(newPathWithFolder);

                if (!string.IsNullOrEmpty(oldPath))
                {
                    _fileService.DeleteFile(oldPath);
                }
            }

            profile.UpdateBio(dto.Bio);

            await _unitOfWork.CompleteAsync();

            var fullName = await _unitOfWork.UserProfiles.GetFullNameAsync(userId);

            var resultDto = _mapper.Map<UserProfileReadDto>(profile);
            resultDto.FullName = fullName;

            return resultDto;

        }
    }
}
