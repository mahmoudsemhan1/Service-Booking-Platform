using Application.Common.page;
using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using Domain.Constants;
using Domain.Interfaces.UnitofWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Service_Booking_Platform.Controllers
{
    /// <summary>
    /// Handles service booking operations including creation, lifecycle management (confirm, cancel, complete), and filtering.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {

            _bookingService = bookingService;
        }
        /// <summary>
        /// Retrieves the booking history for the currently authenticated user.
        /// </summary>
        /// <param name="paging">Pagination parameters (PageNumber, PageSize).</param>
        /// <returns>A paged list of the user's bookings.</returns>
        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings([FromQuery] PaginationParams paging)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _bookingService.GetUserBookingsAsync(userId, paging);

            return Ok(result);
        }
        /// <summary>
        /// Filters and retrieves all bookings (Accessible by Admins and Providers).
        /// </summary>
        /// <param name="filterDto">Criteria to filter bookings (e.g., Status, Date range).</param>
        /// <returns>A list of filtered bookings.</returns>
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.Provider)]
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

        /// <summary>
        /// Retrieves the details of a specific booking by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the booking.</param>
        /// <returns>Booking details if found.</returns>
        /// <response code="200">Returns the requested booking.</response>
        /// <response code="404">If the booking does not exist.</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
                return NotFound();
            return Ok(booking);
        }
        /// <summary>
        /// Creates a new service booking.
        /// </summary>
        /// <remarks>
        /// Only users with the "User" role can create bookings.
        /// </remarks>
        /// <param name="bookingdto">Booking details (ServiceId, DateTime, etc.).</param>
        /// <returns>The newly created booking object.</returns>
        [Authorize(Roles = AppRoles.User)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreatBooking([FromBody] BookingCreateDto bookingdto)
        {
            // if (!ModelState.IsValid) return BadRequest(ModelState);  => ModelState.IsValid automatic check in api controller 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var createdBooking = await _bookingService.CreateAsync(bookingdto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdBooking.Id }, createdBooking);
        }


        /// <summary>
        /// Updates an existing booking's information.
        /// </summary>
        /// <param name="id">The ID of the booking to update.</param>
        /// <param name="bookingdto">Updated booking data.</param>
        [Authorize(Roles = AppRoles.Provider)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto bookingdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var booking = await _bookingService.UpdateAsync(id, bookingdto);

            return Ok(booking);

        }
        /// <summary>
        /// Deletes a booking record (Accessible by Owner or Admin).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.User} , {AppRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            await _bookingService.DeleteAsync(id, userId!, userRole!);

            return NoContent();
        }
        /// <summary>
        /// Confirms a pending booking.
        /// </summary>
        /// <remarks>
        /// Executed by the Provider to accept the booking request.
        /// </remarks>
        [HttpPost("{id}/confirm")]
        [Authorize(Roles = AppRoles.Provider)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Cancels a booking. 
        /// </summary>
        /// <remarks>
        /// Can be initiated by the User, Provider, or Admin depending on the current state.
        /// </remarks>
        [HttpPost("{id}/cancel")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

          
                var success = await _bookingService.CancelAsync(id, userId!, userRole!);
                if (!success) return BadRequest(new { message = "Booking cannot be cancelled in its current state." });

            return NoContent();
        }

        /// <summary>
        /// Marks a booking as completed after the service is rendered.
        /// </summary>
        [HttpPost("{id}/complete")]
        [Authorize(Roles = AppRoles.Provider)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Complete(int id)
        {
            var providerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var success = await _bookingService.CompleteAsync(id, providerUserId!);
                return success ? NoContent() : BadRequest(new { message = "Booking cannot be completed." });
  
        }

    }
}

