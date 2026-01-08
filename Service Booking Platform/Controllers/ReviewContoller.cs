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
        private readonly IUnitofWork _unitofwork;
        private readonly IReviewService _reviewService;

        public ReviewContoller(IUnitofWork unitofwork, IReviewService reviewService)
        {
            _unitofwork = unitofwork;
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Rewvies = await _unitofwork.Reviews.GetAllAsync();
            if (Rewvies == null)
                return NotFound();
            return Ok(Rewvies);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Review = await _unitofwork.Reviews.GetByIdAsync(id);
            if (Review == null)
                return NotFound();
            return Ok(Review);
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
