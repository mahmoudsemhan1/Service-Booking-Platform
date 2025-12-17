using Application.DTOs.Payment;


namespace Application.Interfaces.Services.IPaymentService
{
    public interface IPaymentService
    {
        Task<PaymentReadDto> CreateAsync(PaymentCreateDto dto);
        Task<PaymentReadDto?> GetByIdAsync(int paymentId);
        Task<IEnumerable<PaymentReadDto>> GetAllAsync();


        //Payment Lifecycle (Domain Logic)
        Task<bool> MarkAsSuccessAsync(PaymentUpdateStatusDto dto);
        Task<bool> MarkAsFailedAsync(PaymentUpdateStatusDto dto);
        Task<bool> RefundAsync(int paymentId);





    }
}
