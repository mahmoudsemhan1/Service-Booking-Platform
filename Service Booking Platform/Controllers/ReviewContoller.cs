using Application.Common.page;
using Application.DTOs.Review;
using Application.Interfaces.Services.IReviewServices;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Service_Booking_Platform.Controllers
{
    /// <summary>
    /// Handles user reviews and ratings for service providers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewContoller : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewContoller(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }


        /// <summary>
        /// Retrieves a paged list of reviews for a specific provider.
        /// </summary>
        /// <param name="providerId">The unique ID of the provider.</param>
        /// <param name="paging">Pagination parameters (PageNumber, PageSize).</param>
        /// <returns>A paged list of reviews including ratings and comments.</returns>
        /// <response code="200">Returns the list of reviews.</response>
        [Authorize]
        [HttpGet("provider/{providerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReviews(int providerId,[FromQuery] PaginationParams paging)
        {
            var reviews = await _reviewService.GetServiceReviewsAsync(providerId,paging);
            if (reviews.TotalCount == 0)
            {
                return Ok(new { message = "There are no reviews currently", data = reviews });
            }
            return Ok(reviews);
        }
        /// <summary>
        /// Submits a new review for a completed service.
        /// </summary>
        /// <remarks>
        /// Users can only review providers after a booking has been completed.
        /// The rating should typically be between 1 and 5.
        /// </remarks>
        /// <param name="dto">The review data (Rating, Comment, ProviderId/BookingId).</param>
        /// <returns>A success message and the ID of the created review.</returns>
        /// <response code="200">Review submitted successfully.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="400">If the review data is invalid or doesn't meet business rules.</response>
        [Authorize]
        [HttpPost("add-review")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddReview(CreateReviewDto dto)
        {
            var userid =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userid == null)
                return Unauthorized();

            var reviewId = await _reviewService.AddReviewAsync(userid, dto);

            return Ok(new { message = "Rating added successfully", ReviewId = reviewId });

        }
    }
}
