
using Application.DTOs.Payment;
using Application.Interfaces.Services.IPaymentService;
using AutoMapper;
using Domain.Interfaces.UnitofWork;
using Domain.Models;

namespace Application.Interfaces.Services.Implement
{
    public class PaymentService : Application.Interfaces.Services.IPaymentService.IPaymentService
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
            var payments =await _unitofWork.Payments.GetAllAsync();
            return  _mapper.Map<IEnumerable<PaymentReadDto>>(payments);
        }

        public async Task<PaymentReadDto?> GetByIdAsync(int paymentId)
        {
            var paymrnt= await _unitofWork.Payments.GetByIdAsync(paymentId);

            return paymrnt == null ? null : _mapper.Map<PaymentReadDto>(paymrnt);
        }

        public async Task<PaymentReadDto> CreateAsync(PaymentCreateDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            await _unitofWork.Payments.AddAsync(payment);
            await _unitofWork.CompleteAsync();
            return _mapper.Map<PaymentReadDto>(payment);

        }
        public async Task<bool> MarkAsSuccessAsync(PaymentUpdateStatusDto dto)
        {
            var payment= await _unitofWork.Payments.GetByTransactionIdAsync(dto.TransactionId);
            if(payment == null) return false;

            payment.MarkAsPaid(dto.TransactionId);
            await _unitofWork.CompleteAsync();
            return true;
        }

        public async Task<bool> MarkAsFailedAsync(PaymentUpdateStatusDto dto)
        {
            var payment = await _unitofWork.Payments.GetByTransactionIdAsync(dto.TransactionId);
            if (payment == null) return false;
            
            payment.MarkAsFailed(dto.RawResponse);
            await _unitofWork.CompleteAsync();
            return true;


        }
        public async Task<bool> RefundAsync(int paymentId)
        {
            var payment = await _unitofWork.Payments.GetByIdAsync(paymentId);
            if (payment == null) return false;

            payment.MarkAsFailed();
            await _unitofWork.CompleteAsync();
            return true;
        }
    }
}
