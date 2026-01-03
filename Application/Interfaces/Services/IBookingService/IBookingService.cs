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
        Task<bool> ConfirmAsync(int bookingId);
        Task<bool> CancelAsync(int bookingId);
        Task<bool> CompleteAsync(int bookingId);
        Task<IEnumerable<BookingReadDto>> GetAllAsync(BookingFilterDto filter);

        Task<BookingReadDto?> GetByIdAsync(int bookingId);

        Task<BookingReadDto> CreateAsync(BookingCreateDto dto, string currentUserId);
        Task<BookingReadDto> UpdateAsync(int bookingId, BookingUpdateDto dto);
        Task DeleteAsync(int bookingId);


    }
}
