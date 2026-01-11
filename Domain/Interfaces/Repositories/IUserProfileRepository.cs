using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<string?> GetFullNameAsync(string userId);

    }
}
