using Application.Common.Models;
using Application.DTOs.Review;

namespace Application.Interfaces.Services.IReviewServices
{
    public interface IReviewService
    {
        Task<int> AddReviewAsync(string userId, CreateReviewDto dto);
    }
}
