using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using Domain.Constants;
using Domain.Interfaces.UnitofWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {

            _bookingService = bookingService;
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> GetAll([FromQuery] BookingFilterDto filterDto)
        {
            var booking = await _bookingService.GetAllAsync(filterDto);
            return Ok(booking);
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var booking = await _unitofWork.Bookings.GetAllAsync() ;
        //    return Ok(booking);
        //}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
                return NotFound();
            return Ok(booking);
        }
        [Authorize(Roles = AppRoles.User)]
        [HttpPost]
        public async Task<IActionResult> CreatBooking([FromBody] BookingCreateDto bookingdto)
        {
            // if (!ModelState.IsValid) return BadRequest(ModelState);  => ModelState.IsValid automatic check in api controller 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var createdBooking = await _bookingService.CreateAsync(bookingdto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdBooking.Id }, createdBooking);
        }


        
        [Authorize(Roles = AppRoles.Provider)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto bookingdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var booking = await _bookingService.UpdateAsync(id, bookingdto);

            return Ok(booking);

        }
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.User} , {AppRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            await _bookingService.DeleteAsync(id, userId!, userRole!);

            return NoContent();
        }
        [HttpPost("{id}/confirm")]
        [Authorize(Roles = AppRoles.Provider)]
        public async Task<IActionResult> Confirm(int id)
        {
            var providerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(providerUserId)) return Unauthorized();

            try
            {
                var success = await _bookingService.ConfirmAsync(id, providerUserId);

                if (!success)
                    return BadRequest(new { message = "Booking not found or already confirmed." });

                return NoContent();
            }
            catch (Exception )
            {
                // ده هيخلي الميدل وير يرجع الرسالة اللي في الـ Domain (زي: Cannot confirm. Current status is...)
                throw;
            }

        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

          
                var success = await _bookingService.CancelAsync(id, userId!, userRole!);
                if (!success) return BadRequest(new { message = "Booking cannot be cancelled in its current state." });

            return NoContent();
        }

        [HttpPost("{id}/complete")]
        [Authorize(Roles = AppRoles.Provider)]
        public async Task<IActionResult> Complete(int id)
        {
            var providerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var success = await _bookingService.CompleteAsync(id, providerUserId!);
                return success ? NoContent() : BadRequest(new { message = "Booking cannot be completed." });
  
        }

    }
}

