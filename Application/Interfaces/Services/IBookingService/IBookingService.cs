using Application.DTOs.Booking;
using Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.BookingService
{
    public interface IBookingService
    {
        Task<BookingReadDto?> GetByIdAsync(int id);
        Task<IEnumerable<BookingReadDto>> GetAllAsync();
        Task<BookingCreateDto> CreateAsync(BookingCreateDto dto);
        Task<BookingUpdateDto> UpdateAsync(int bookingId, BookingUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
