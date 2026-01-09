using Application.DTOs.Review;
using Domain.Models;
using Domain.Models.Views;

namespace Application.Interfaces.Services.IReviewServices
{
    public interface IReviewService
    {
        Task<int> AddReviewAsync(string userId, CreateReviewDto dto);
        Task<IEnumerable<ProviderReviewView>> GetProviderReviewsAsync(int providerId);
    }
}
