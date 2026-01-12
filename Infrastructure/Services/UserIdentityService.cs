using Application.Common.page;
using Application.DTOs.Account;
using Application.DTOs.Paged;
using Application.Interfaces.Services.IUserIdentityServices;
using AutoMapper;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserIdentityService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<UserReadDto>> GetAllUsersPagedAsync(PaginationParams paging)
        {
            var query = _userManager.Users.AsNoTracking();

            //search filter by username or email
            if (!string.IsNullOrWhiteSpace(paging.Search))
            {
                var search = paging.Search.Trim().ToLower();
                query = query.Where(u => u.UserName.ToLower().Contains(search) ||
                                         u.Email.ToLower().Contains(search));
            }
            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((paging.PageNumber - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            var userDtos = new List<UserReadDto>();
            foreach (var user in users)
            {
                var dto = new UserReadDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsEmailConfirmed = user.EmailConfirmed,
                };

                // جلب الأدوار
                var roles = await _userManager.GetRolesAsync(user);
                dto.Roles = roles.ToList();

                userDtos.Add(dto);
            }

            return new PagedResultDto<UserReadDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
           var user = await _userManager.FindByIdAsync(userId);
           
            return user?.UserName ?? "User";
        }
    }
}
