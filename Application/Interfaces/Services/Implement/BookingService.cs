using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using AutoMapper;
using Domain.Constants;
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

        public async Task<bool> ConfirmAsync(int bookingId , string providerUserId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || booking.Status != BookingStatus.Pending)
                return false;

            if (booking.Status != BookingStatus.Pending) return false;

            booking.Confirm();
            booking.Payment = new Payment(booking.Id, booking.UserId, booking.TotalPrice, PaymentMethod.Unknown);

            

            await _unitOfWork.CompleteAsync();
            return true;

        }

        public async Task<bool> CancelAsync(int bookingId,string userId, string userRole)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) return false;

            //ownership check
            bool isOwner = booking.UserId == userId;
            bool isProvider = booking.ProviderId.ToString() == userId; 
            bool isAdmin = userRole == AppRoles.Admin;
            if(!isOwner && !isProvider && !isAdmin) 
                throw new InvalidOperationException("Cannot cancel a confirmed booking. Please contact the provider.");

          

            booking.Cancel();
            await _unitOfWork.CompleteAsync(); 
            return true;
        }

        public async Task<bool> CompleteAsync(int bookingId, string providerUserId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(bookingId);

            if (booking == null) return false;

            if (booking.Provider.UserId != providerUserId)
                throw new UnauthorizedAccessException("You are not authorized to complete this booking.");

            if (booking.Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be marked as completed.");

            booking.Complete();

            await _unitOfWork.CompleteAsync();
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
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);

            return _mapper.Map<BookingReadDto>(booking);
        }

        public async Task<BookingReadDto> CreateAsync(BookingCreateDto dto, string currentUserId)
        {
            var providerService = (await _unitOfWork.ProviderServices.FindAsync(ps =>
            ps.ProviderId == dto.ProviderId &&
            ps.ServiceId == dto.ServiceId &&
            !ps.IsDeleted
            )).FirstOrDefault();
            if (providerService != null)
            {
                throw new Exception("This provider does not currently offer this service   ");
            }
            var provider = await _unitOfWork.Providers.GetByIdAsync(dto.ProviderId);
            if (provider != null && provider.UserId == currentUserId)
                throw new InvalidOperationException("Providers cannot book their own services.");


         var isSlotOccupied = (await _unitOfWork.Bookings.FindAsync(b =>
                 b.ProviderId == dto.ProviderId &&
                 b.BookingDate.Date == dto.BookingDate.Date &&
                 b.BookingTime == dto.BookingTime &&
                 b.Status != BookingStatus.Cancelled)).Any();
            if (isSlotOccupied)
                throw new InvalidOperationException("The selected time slot is already booked. Please choose another time.");

            var booking = new Booking(currentUserId, dto.ServiceId, dto.ProviderId)
            {
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                // Price snapshot (fixing the price at the time of booking) to ensure it is not affected by subsequent price changes.
                TotalPrice = providerService.DiscountedPrice ?? providerService.Price,
                CreatedAt = DateTime.UtcNow
            };


            await _unitOfWork.Bookings.AddAsync(booking);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) throw new Exception("An error occurred while saving the booking.");

            var createdBooking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(booking.Id);
            return _mapper.Map<BookingReadDto>(createdBooking);


        }

        public async Task<BookingReadDto> UpdateAsync(int bookingId, BookingUpdateDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) throw new KeyNotFoundException("Booking not found");

            _mapper.Map(dto, booking);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookingReadDto>(booking);

        }

        public async Task DeleteAsync(int bookingId , string userId, string userRole)
        {

            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) throw new KeyNotFoundException("Booking not found");

            if (booking.UserId != userId && userRole != AppRoles.Admin)
                throw new UnauthorizedAccessException("You can only delete your own bookings.");

            if (booking.Status != BookingStatus.Pending && userRole != AppRoles.Admin)
                throw new InvalidOperationException("Confirmed bookings cannot be deleted.");

            await _unitOfWork.Bookings.DeleteAsync(booking);
            await _unitOfWork.CompleteAsync();





        }
    }
}
