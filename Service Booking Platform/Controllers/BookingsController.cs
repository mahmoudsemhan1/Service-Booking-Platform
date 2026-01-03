using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
 using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IUnitofWork _unitofWork;

        public BookingsController( IBookingService bookingService, IUnitofWork unitofWork)
        {
           
            _bookingService = bookingService;
            _unitofWork = unitofWork;
        }

        [HttpGet("Fileter")]
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
        public async Task<IActionResult> GetById(int id)
        {
            var booking=await _bookingService.GetByIdAsync(id);
            return Ok(booking);
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreatBooking([FromBody] BookingCreateDto bookingdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(userId==null) return BadRequest(ModelState);

                var createdBooking = await _bookingService.CreateAsync(bookingdto,userId);
                return CreatedAtAction(nameof(GetById), new { id = createdBooking.Id }, createdBooking);
            }
            catch (Exception ex )
            {
                return BadRequest(new {message=ex.Message});
            }


        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto bookingdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var booking = await _bookingService.UpdateAsync(id, bookingdto);

            return Ok(booking);
        
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
             await _bookingService.DeleteAsync(id);

            return NoContent();
        }
        [HttpPost("{id}/confirm")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> Confirm(int id)
        {
            var success = await _bookingService.ConfirmAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _bookingService.CancelAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> Complete(int id)
        {
            var success = await _bookingService.CompleteAsync(id);
            return success ? NoContent() : NotFound();
        }

    }
}

