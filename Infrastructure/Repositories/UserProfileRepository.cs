using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileRepository(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

        }

        public async Task<string?> GetFullNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.FullName;
        }
    }
}
