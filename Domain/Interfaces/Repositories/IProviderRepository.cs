using Domain.Models;


namespace Domain.Interfaces.Repositories
{
    public interface IProviderRepository : IGenericRepository<Provider>
    {
        //
        Task<Provider?> GetByUserIdWithDetailsAsync(string userId);
    }
}
