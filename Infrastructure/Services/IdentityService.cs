using Application.DTOs.Identity;
using Application.Interfaces.Services.Identity;
using AutoMapper;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly IMapper _mapper;


        public IdentityService(UserManager<ApplicationUser> userManager, ILogger<IdentityService> logger, IMapper mapper)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AppUserReadDto> CreateUserAsync(AppUserCreateDto dto)
        {
            // check if user exists 
            var existsuser= await _userManager.FindByEmailAsync(dto.Email);
            if (existsuser != null)
                throw new ApplicationException($"User with email '{dto.Email}' already exists.");

            // Create new user
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.Phone,
                PhotoPath = dto.PhotoPath,
                EmailConfirmed = true // Set to false if using email confirmation
            };

            var result = await _userManager.CreateAsync(user,dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user: {Errors}", errors);
                throw new ApplicationException($"Failed to create user: {errors}");
            }
            // Assign default role if needed
            await _userManager.AddToRoleAsync(user, "User");

            return _mapper.Map<AppUserReadDto>(user);

        }
        public Task<bool> AssignRoleAsync(string userId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<string> AuthenticateAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }


        public Task<bool> DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUserReadDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AppUserReadDto?> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<AppUserReadDto?> GetUserByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveRoleAsync(string userId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserAsync(string id, AppUserUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
