using Application.DTOs.Payment;
using Application.Interfaces.Services.IPaymentService;
using Microsoft.AspNetCore.Mvc;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {

        private readonly IPaymentService _paymentService;

        public WebhookController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("success")]
        public async Task<IActionResult> MarkAsSuccess([FromBody] PaymentUpdateStatusDto dto)
        {
            var result = await _paymentService.MarkAsSuccessAsync(dto);
            if (!result) return NotFound();


            return Ok("Payment marked as success and booking completed");
        }

        [HttpPost("failed")]
        public async Task<IActionResult> MarkAsFailed([FromBody] PaymentUpdateStatusDto dto)
        {
            var result = await _paymentService.MarkAsFailedAsync(dto);
            if (!result) return NotFound("Payment Not found");

            return Ok("Payment marked as failed");
        }
    }
}
