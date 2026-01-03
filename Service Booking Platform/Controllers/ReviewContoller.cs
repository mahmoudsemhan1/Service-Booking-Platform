using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewContoller : ControllerBase
    {
        private readonly IUnitofWork _unitofwork;

        public ReviewContoller(IUnitofWork unitofwork)
        {
            _unitofwork = unitofwork;
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
            if(Review == null) 
                return NotFound();
            return Ok(Review);
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateReview(Review review)
        //{
            
        //}
    }
}
