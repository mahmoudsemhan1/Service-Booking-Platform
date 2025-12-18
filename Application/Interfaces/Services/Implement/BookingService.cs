using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Domain.Models.Enum;


namespace Application.Interfaces.Services.Implement
{
    public class BookingService : IBookingService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitOfWork = unitofWork;
            _mapper = mapper;
        }

        public async Task<bool> ConfirmAsync(int bookingId) 
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if(booking==null || booking.Status != BookingStatus.Pending) 
                return false;

            booking.Confirm();
            var payment = new Payment
                (
               bookingId: booking.Id,
               userId: booking.UserId,
               amount: booking.TotalPrice,
               method: PaymentMethod.Unknown 
           );
            await _unitOfWork.Payments.AddAsync(payment);
            
            //ربط payment بال booking
            booking.PaymentId = payment.Id;
            booking.Payment = payment;

            await _unitOfWork.CompleteAsync(); 


            return true;

        }

        public async Task<bool> CancelAsync(int bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (
                booking == null ||
                booking.Status == BookingStatus.Completed|| 
                booking.Status==BookingStatus.Cancelled
                )
                return false;

            booking.Cancel();
            await _unitOfWork.CompleteAsync(); // حفظ التغييرات
            return true;
        }

        public async Task<bool> CompleteAsync(int bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null || booking.Status != BookingStatus.Confirmed)
                return false;

            booking.Complete();
            await _unitOfWork.CompleteAsync(); // حفظ التغييرات
            return true;
        }

        public async Task<IEnumerable<BookingReadDto>> GetAllAsync(BookingFilterDto filter)
        {
            var bookings = await _unitOfWork.Bookings.GetAsync
                (
                filter.ServiceId, 
                filter.ProviderId,
                filter.Status,
                filter.FromDate,
                filter.ToDate
                );

            return _mapper.Map<IEnumerable<BookingReadDto>>(bookings);

        }

        public async Task<BookingReadDto?> GetByIdAsync(int bookingId)
        {
           var booking= await _unitOfWork.Bookings.GetByIdAsync (bookingId);

            return _mapper.Map<BookingReadDto>(booking);
        }

        public async Task<BookingReadDto> CreateAsync(BookingCreateDto dto)
        {
            var booking= _mapper.Map<Booking>(dto);
           
            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookingReadDto>(booking);


        }

        public async Task<BookingReadDto> UpdateAsync(int bookingId, BookingUpdateDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) throw new KeyNotFoundException("Booking not found");

            _mapper.Map(dto, booking);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookingReadDto>(booking);

        }

        public async Task DeleteAsync(int bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) throw new KeyNotFoundException("Booking not found");

             await _unitOfWork.Bookings.DeleteAsync(booking);
            await _unitOfWork.CompleteAsync();





        }
    }
}
