using Application.DTOs.Payment;


namespace Application.Interfaces.Services.IPaymentService
{
    public interface IPaymentService
    {
        Task<PaymentReadDto> CreateAsync(PaymentCreateDto dto,string userId);
        Task<PaymentReadDto?> GetByIdAsync(int paymentId);
        Task<IEnumerable<PaymentReadDto>> GetAllAsync();


        //Payment Lifecycle (Domain Logic)
        //change from bool to void and throw exception in case of error , because the midelleware will handle it
        Task MarkAsSuccessAsync(PaymentUpdateStatusDto dto);
        Task MarkAsFailedAsync(PaymentUpdateStatusDto dto);
        Task RefundAsync(int paymentId);

        // Stripe Specific Methods 
        // Create Checkout Session and Handle Webhook Events
        // Return the session URL to redirect the user to Stripe Checkout => this the aime of this method
        Task<string> CreateCheckoutSessionAsync(int bookingId, string userId);
        //  Handle successful payment webhook event
        Task HandlePaymentSuccessAsync(string sessionId);





    }
}
