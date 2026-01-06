using Application.DTOs.Booking;
using Application.Interfaces.Services.BookingService;
using Application.Interfaces.Services.IUserIdentityServices;
using AutoMapper;
using Domain.Constants;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Domain.Models.Enum;
using Microsoft.EntityFrameworkCore;


namespace Application.Interfaces.Services.Implement
{
    public class BookingService : IBookingService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserIdentityService _userIdentityService;
        public BookingService(IUnitofWork unitofWork, IMapper mapper, IUserIdentityService userIdentityService)
        {
            _unitOfWork = unitofWork;
            _mapper = mapper;
            _userIdentityService = userIdentityService;
        }

        public async Task<bool> ConfirmAsync(int bookingId, string providerUserId)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) return false;

            booking.Confirm();

            if (booking.Payment == null)
            {
                booking.Payment = new Payment(booking.Id, booking.UserId, booking.TotalPrice, PaymentMethod.Unknown);
            }

              await _unitOfWork.Bookings.UpdateAsync(booking);

            //
            var result = await _unitOfWork.CompleteAsync();

            return true;

        }

        public async Task<bool> CancelAsync(int bookingId, string userId, string userRole)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) return false;

            //ownership check
            bool isOwner = booking.UserId == userId;
            bool isProvider = booking.ProviderId.ToString() == userId;
            bool isAdmin = userRole == AppRoles.Admin;
            if (!isOwner && !isProvider && !isAdmin)
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
            // 1. التحقق من توافر الخدمة والمزود (Logic السابق سليم)
            var providerService = (await _unitOfWork.ProviderServices.FindAsync(ps =>
                ps.ProviderId == dto.ProviderId &&
                ps.ServiceId == dto.ServiceId &&
                !ps.IsDeleted
            )).FirstOrDefault();

            if (providerService == null)
                throw new Exception("This provider does not currently offer this service.");

            // 2. إنشاء كائن الحجز
            var booking = new Booking(currentUserId, dto.ServiceId, dto.ProviderId)
            {
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                TotalPrice = providerService.DiscountedPrice ?? providerService.Price,
                CreatedAt = DateTime.UtcNow
            };

            // 3. الحفظ في قاعدة البيانات
            try
            {
                await _unitOfWork.Bookings.AddAsync(booking);
                var result = await _unitOfWork.CompleteAsync();
                // ... الباقي
            }
            catch (DbUpdateException dbEx)
            {
                // ده هيطلعلك السبب التقني (مثلاً Foreign Key Conflict)
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"Database Detail: {innerMessage}");
            }
            // هنا ممكن يحصل Error لو فيه Database Constraint (زي ForeignKey غلط)

            // التحقق من الحفظ
            //if (result <= 0)
            //{
            //    // بدل الـ Exception العام، هنحاول نعرف ليه مفيش داتا اتحفظت
            //    throw new Exception("Database Save Failed: No rows were affected. Check constraints or Database connection.");
            //}

            // 4. جلب البيانات بالـ Includes
            // تأكد إن الميثود دي في الـ Repository مش بترجع null
            var resultWithDetails = await _unitOfWork.Bookings.GetByIdWithDetailsAsync(booking.Id);

            if (resultWithDetails == null)
            {
                throw new Exception($"Booking {booking.Id} saved, but could not be re-loaded from database.");
            }

            // 5. الـ Mapping والـ User Identity
            var bookingReadDto = _mapper.Map<BookingReadDto>(resultWithDetails);

            try
            {
                // استخدمنا try-catch هنا عشان لو خدمة اليوزر وقعت، الحجز ميفشلش كله
                if (_userIdentityService != null)
                {
                    bookingReadDto.UserName = await _userIdentityService.GetUserNameAsync(currentUserId);
                }
            }
            catch (Exception )
            {
                // سجل الخطأ لكن كمل العملية
                bookingReadDto.UserName = "User Name Unavailable";
            }

            return bookingReadDto;
        }

        public async Task<BookingReadDto> UpdateAsync(int bookingId, BookingUpdateDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null) throw new KeyNotFoundException("Booking not found");

            _mapper.Map(dto, booking);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookingReadDto>(booking);

        }

        public async Task DeleteAsync(int bookingId, string userId, string userRole)
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
