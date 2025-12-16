using Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Identity
{
    public interface IIdentityService
    {
        // User Management
        Task<AppUserReadDto> CreateUserAsync(AppUserCreateDto dto);
        Task<AppUserReadDto?> GetUserByIdAsync(string id);
        Task<AppUserReadDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<AppUserReadDto>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(string id, AppUserUpdateDto dto);
        Task<bool> DeleteUserAsync(string id);

        // Authentication
        Task<string> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        // Roles
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> RemoveRoleAsync(string userId, string role);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);

    }
}
