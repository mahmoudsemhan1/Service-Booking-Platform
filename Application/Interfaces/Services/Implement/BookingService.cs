using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Implement
{
    public class BookingService : IBookingService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<BookingReadDto>> GetAllAsync()
        {
            var booking = await _unitofWork.Bookings.GetAllAsync();
 
            return _mapper.Map<IEnumerable<BookingReadDto>>(booking);

        }

        public async Task<BookingReadDto?> GetByIdAsync(int id)
        {
             var booking = await _unitofWork.Bookings.GetByIdAsync(id);
            if (booking == null) return null;

           return  _mapper.Map<BookingReadDto>(booking);
            
        }

        public async Task<BookingCreateDto> CreateAsync(BookingCreateDto dto)
        {
             // تحقق من وجود User
            var user = (await _unitofWork.Users.FindAsync(b=>b.Id==dto.UserId)).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found");

            // تحقق من وجود Provider
            var provider = await _unitofWork.Providers.GetByIdAsync(dto.ProviderId); 
            if (provider == null)
                throw new Exception("Provider not found");
            // تحقق من وجود Service
            var service= await _unitofWork.Services.GetByIdAsync(dto.ServiceId);
            if (service == null)
                throw new Exception("Service not found");
            // حساب السعر (يمكن إضافة أي خصومات أو رسوم إضافية هنا)
            var totalprice= service.Price;
            var booking = new Booking
            {
                UserId = dto.UserId,
                ProviderId = dto.ProviderId,
                ServiceId = dto.ServiceId,
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                TotalPrice = totalprice,
                Status = BookingStatus.Pending
            };
            await _unitofWork.Bookings.AddAsync(booking);
            await _unitofWork.SaveAsync();

            return _mapper.Map<BookingCreateDto>(booking);
        }
        public async Task<BookingUpdateDto> UpdateAsync(int bookingId, BookingUpdateDto dto)
        {
            var existingBooking = await _unitofWork.Bookings.GetByIdAsync(bookingId);
            if (existingBooking == null)
                throw new ArgumentException($"Booking with id {bookingId} not found", nameof(bookingId));

            _mapper.Map(dto, existingBooking);
            await _unitofWork.Bookings.UpdateAsync(existingBooking);
            await _unitofWork.SaveAsync();

            return _mapper.Map<BookingUpdateDto>(existingBooking);


        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _unitofWork.Bookings.GetByIdAsync(id);
            if (booking == null) return false;

            await _unitofWork.Bookings.DeleteAsync(booking);
            await _unitofWork.SaveAsync();
            return true;
        }


    }
}
