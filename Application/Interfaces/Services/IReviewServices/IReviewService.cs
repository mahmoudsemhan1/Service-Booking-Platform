using Application.Common.page;
using Application.DTOs.Paged;
using Application.DTOs.Review;
using Domain.Models;
using Domain.Models.Views;

namespace Application.Interfaces.Services.IReviewServices
{
    public interface IReviewService
    {
        Task<int> AddReviewAsync(string userId, CreateReviewDto dto);
        Task<IEnumerable<ProviderReviewView>> GetProviderReviewsAsync(int providerId);

        //this for pagination
        Task<PagedResultDto<ReadReviewDto>> GetServiceReviewsAsync(int serviceId, PaginationParams paginge);

    }
}
