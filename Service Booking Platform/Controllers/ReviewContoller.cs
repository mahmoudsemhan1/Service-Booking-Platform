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
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewContoller : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewContoller(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpGet("provider/{providerId}")]
        public async Task<IActionResult> GetReviews(int providerId)
        {
            var reviews = await _reviewService.GetProviderReviewsAsync(providerId);

            if (reviews == null || !reviews.Any())
                return Ok(new { message = "There are no reviews currently", data = reviews });

            return Ok(reviews);
        }
        [Authorize]
        [HttpPost("add-review")]
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
