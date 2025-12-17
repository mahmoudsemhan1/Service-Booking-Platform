using Application.DTOs.Payment;
using Application.Interfaces.Services.IPaymentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var Payments = await _paymentService.GetAllAsync();
            if (Payments == null)
                return NotFound();
            return Ok(Payments);

        }
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);
            if (payment == null) return NotFound();

            return Ok(payment);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = await _paymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPaymentById), new { paymentId = payment.Id }, payment);

        }

        [HttpPost("success")]
        public async Task<IActionResult> MarkAsSuccess([FromBody] PaymentUpdateStatusDto dto)
        {
            var result = await _paymentService.MarkAsSuccessAsync(dto);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPost("failed")]
        public async Task<IActionResult> MarkAsFailed([FromBody] PaymentUpdateStatusDto dto)
        {
            var result = await _paymentService.MarkAsFailedAsync(dto);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPost("{paymentId}/refund")]
        public async Task<IActionResult> RefundPayment(int paymentId)
        {
            var result = await _paymentService.RefundAsync(paymentId);
            if (!result) return NotFound();
            return Ok();
        }


    }
}
