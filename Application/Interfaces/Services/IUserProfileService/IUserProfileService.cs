using Application.DTOs.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.IUserProfileService
{
    public interface IUserProfileService
    {
        Task<UserProfileReadDto> UpdateAsync(string userId, UpdateProfileDto dto);
        Task<UserProfileReadDto> GetByUserIdAsync(string userId);
    }
}
