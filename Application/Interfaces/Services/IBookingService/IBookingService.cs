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
        Task<bool> CancelAsync(int bookingId, string userId, string userRole);
        Task DeleteAsync(int bookingId, string userId, string userRole);
        Task<bool> ConfirmAsync(int bookingId, string providerUserId);
        Task<bool> CompleteAsync(int bookingId, string providerUserId);
        Task<IEnumerable<BookingReadDto>> GetAllAsync(BookingFilterDto filter);

        Task<BookingReadDto?> GetByIdAsync(int bookingId);

        Task<BookingReadDto> CreateAsync(BookingCreateDto dto, string currentUserId);
        Task<BookingReadDto> UpdateAsync(int bookingId, BookingUpdateDto dto);


    }
}
