using Application.Common.page;
using Application.DTOs.Account;
using Application.DTOs.Paged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.IUserIdentityServices
{
    public interface IUserIdentityService
    {
        Task<string> GetUserNameAsync(string userId);
        Task<PagedResultDto<UserReadDto>> GetAllUsersPagedAsync(PaginationParams paging);
    }
}
