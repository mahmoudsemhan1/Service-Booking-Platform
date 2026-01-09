using Domain.Models;
using Domain.Models.Views;


namespace Domain.Interfaces.Repositories
{
    public interface IReviewRepository:IGenericRepository<Review>
    {
        Task<IEnumerable<ProviderReviewView>> GetProviderReviewsAsync(int providerId);


    }

}
