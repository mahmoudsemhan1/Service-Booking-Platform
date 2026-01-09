using Application.Common.Models;
using Application.DTOs.Review;
using Application.Interfaces.Services.IReviewServices;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Domain.Models.Enum;
using Domain.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Implement
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitofWork  _unitofWork ;
        private readonly IMapper _mapper;

        public ReviewService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;
        }

        public async Task<int> AddReviewAsync(string userId, CreateReviewDto dto)
        {
            // Get booking with Detailies
            var booking = await _unitofWork.Bookings.GetByIdWithDetailsAsync(dto.BookingId);
            // make some check on this booking ,
            // are  not null, and this booking belong for this user ,
            // check if this booking complete or not ,
            // and are this user make review befor or not 
            if (booking == null)
                throw new KeyNotFoundException("Sorry, this booking is not exisit");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException();
            if (booking.Status != BookingStatus.Completed)
                throw new InvalidOperationException("The service can only be evaluated after its implementation is complete.");

            var alreadyReviewed = await _unitofWork.Reviews.AnyAsync(r => r.BookingId == dto.BookingId);
            if (alreadyReviewed)
                throw new InvalidOperationException("I have already rated this booking.");

            // start  the TransAction

            await _unitofWork.BeginTransactionAsync();

            try
            {
                var review = new Review
                    (
                    userId , 
                    booking.ProviderId,
                    booking.ServiceId,
                    booking.Id,
                    dto.Rating,
                    dto.Comment
                    );
                // the update will be in two models
                // 1 the serivce update the rating  in it for this sercie
                // 2 update in the provier , update the rating
                
                booking.Service.UpdateRating(dto.Rating);
                booking.Provider.UpdateRating(dto.Rating);

                //add
                await _unitofWork.Reviews.AddAsync(review);

                //
                await _unitofWork.CommitTransactionAsync(); 

                return review.Id;

            }

            catch (Exception ex)
            {
                await _unitofWork.RollbackTransactionAsync();

                if (ex.InnerException != null)
                {
                    throw new InvalidOperationException($"Database Error: {ex.InnerException.Message}");
                }

                throw;
            }

            // 
        }

        public async Task<IEnumerable<ProviderReviewView>> GetProviderReviewsAsync(int providerId)
        {
            var reviews = await _unitofWork.Reviews.GetProviderReviewsAsync(providerId);
            return reviews;

        }
    }
}
