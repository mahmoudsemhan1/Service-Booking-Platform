using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task<Service?> GetByIdWithImagesAsync(int id);
    }

}
