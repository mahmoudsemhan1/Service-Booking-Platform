using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
         private readonly IMapper _mapper;
        private readonly IBookingService _bookingService;

        public BookingsController(  IMapper mapper, IBookingService bookingService)
        {
             _mapper = mapper;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var booking =await _bookingService.GetAllAsync();
            return Ok(booking);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking=await  _bookingService.GetByIdAsync(id);

            return Ok(booking);

        }
        [HttpPost]
        public async Task<IActionResult> CreatBooking([FromBody] BookingCreateDto bookingdto)
        {
            var booking = await _bookingService.CreateAsync(bookingdto);
            return Ok(booking);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto bookingdto)
        {
            var booking = await _bookingService.UpdateAsync(id, bookingdto);

            return NoContent();
        
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking =await _bookingService.DeleteAsync(id);

            return NoContent();
        }

    }
}

