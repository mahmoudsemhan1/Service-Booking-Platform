
using Application.DTOs.Payment;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Domain.Models.Enum;
using Stripe;
using Stripe.Checkout;

namespace Application.Interfaces.Services.Implement
{
    public class PaymentService : IPaymentService.IPaymentService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PaymentReadDto>> GetAllAsync()
        {
            var payments = await _unitofWork.Payments.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentReadDto>>(payments);
        }

        public async Task<PaymentReadDto?> GetByIdAsync(int paymentId)
        {
            var paymrnt = await _unitofWork.Payments.GetByIdAsync(paymentId);

            return paymrnt == null ? null : _mapper.Map<PaymentReadDto>(paymrnt);
        }

        public async Task<PaymentReadDto> CreateAsync(PaymentCreateDto dto, string userId)
        {
            //first check if booking exists ,and belongs to the user
            var booking = await _unitofWork.Bookings.GetByIdAsync(dto.BookingId);
            if (booking == null || booking.UserId != userId)
                throw new KeyNotFoundException("Booking not found or does not belong to the user.");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to make a payment for this booking.");


            var payment = _mapper.Map<Payment>(dto);

            try
            {
                await _unitofWork.Payments.AddAsync(payment);
                await _unitofWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                // ده هيطلعلك السبب الحقيقي في الـ Response
                var message = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Database Error: {message}");
            }

            return _mapper.Map<PaymentReadDto>(payment);

        }
        public async Task MarkAsSuccessAsync(PaymentUpdateStatusDto dto)
        {
            // Handles payment success coming from payment gateway (Webhook)

            var payment = await _unitofWork.Payments.GetByTransactionIdAsync(dto.TransactionId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with Transaction ID {dto.TransactionId} not found.");

            //this for do the logic in mobile
            payment.MarkAsSuccess(dto.TransactionId, dto.RawResponse);

            //Load related booking
            if (payment.BookingId == null)
                throw new InvalidOperationException("Payment has no related booking");

            var booking = await _unitofWork.Bookings
                .GetByIdAsync(payment.BookingId.Value);

            if (booking == null)
                throw new KeyNotFoundException("Linked booking was not found.");

            //  Complete booking (Domain Logic)
            if (booking.Status == BookingStatus.Confirmed)
                booking.Complete();

            await _unitofWork.CompleteAsync();
        }

        public async Task MarkAsFailedAsync(PaymentUpdateStatusDto dto)
        {
            var payment = await _unitofWork.Payments.GetByTransactionIdAsync(dto.TransactionId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with Transaction ID {dto.TransactionId} not found.");

            payment.MarkAsFailed(dto.RawResponse);
            await _unitofWork.CompleteAsync();

        }
        public async Task RefundAsync(int paymentId)
        {

            var payment = await _unitofWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            if (payment.Status != PaymentStatus.Success)
                throw new InvalidOperationException("Only successful payments can be refunded.");

            // Call Stripe API to process the refund
            var options = new RefundCreateOptions
            {
                PaymentIntent = payment.TransactionId, 
                Reason = RefundReasons.RequestedByCustomer
            };
            var service = new RefundService();

            try
            {
                await service.CreateAsync(options);
                payment.Refund();

                await _unitofWork.CompleteAsync();
            }
            catch (StripeException e)
            {
                throw new Exception($"Refund Failed: {e.StripeError.Message}");
            }
        }

        public async Task<string> CreateCheckoutSessionAsync(int bookingId, string userId)
        {
            // 1- Validate booking exists and belongs to user 
            var booking = await _unitofWork.Bookings.GetByIdAsync(bookingId);
            if (booking == null || booking.UserId != userId)
                throw new KeyNotFoundException("Booking not found or does not belong to the user.");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to make a payment for this booking.");

            //2- Create Stripe Checkout Session
            //  Stripe options and session creation logic would go here.
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = "https://frontend.com/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://frontend.com/cancel",
                //this the metadata that will be reurun to us in the webhook
                Metadata = new Dictionary<string, string>
                {
                      { "BookingId", bookingId.ToString() },
                      { "UserId", userId }
                },
                //this the detailes the user will see it 
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount=(long)(booking.TotalPrice *100),
                            Currency="usd",
                            ProductData= new SessionLineItemPriceDataProductDataOptions
                            {
                                Name= "Service booking payment",
                                Description=$"Payment for Booking {bookingId}"
                            },  
                        },
                        Quantity=1,
                    },
                },
            };
            //-3 ask the strip to create the session 
            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            //-4  record the payment in the database in "pening"
            // wait the wehook to comfirm it 

            var payment = new Payment(bookingId, userId, booking.TotalPrice,Domain.Models.Enum.PaymentMethod.Card);
          //await _unitofWork.Payments.AddAsync(payment);
            await _unitofWork.CompleteAsync();
            //5- Return session URL or ID and this the goool of this method

            return session.Url;
        }



        public async Task HandlePaymentSuccessAsync(string sessionId)
        {
            // Retrieve the session from Stripe to get payment details
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);
            var paymentIntentId = session.PaymentIntentId;
            // get the booking id from metadat
            var bookingId = int.Parse(session.Metadata["BookingId"]);

            var payment = await _unitofWork.Payments.GetByTransactionIdAsync(session.Id); 
            if (payment==null)
                throw new KeyNotFoundException("Payment record not found for this session.");
            //using the MarkAsSuccess

            payment.MarkAsSuccess(sessionId, $"Stripe Session: {sessionId}");
            // update the status of booking  
            

            var booking = await _unitofWork.Bookings.GetByIdAsync(bookingId);
            if (booking != null)
            {
                booking.Confirm();
            }
            await _unitofWork.CompleteAsync();

        }
    }
}
