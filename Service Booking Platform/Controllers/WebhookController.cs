using Application.DTOs.Payment;
using Application.Interfaces.Services.IPaymentService;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public WebhookController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("stripe-success")] 
    public async Task<IActionResult> MarkAsSuccess([FromBody] PaymentUpdateStatusDto dto)
    {
        // 1. استدعاء الميثود مباشرة
        // 2. الـ Service ستتأكد من وجود الدفعة وتحديث الحجز
        // 3. لو الدفعة مش موجودة، الـ Middleware سيرد بـ 404
        await _paymentService.MarkAsSuccessAsync(dto);

        // 4. دائماً نرجع رد JSON منظم
        return Ok(new
        {
            Message = "Payment confirmed and booking completed successfully.",
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpPost("stripe-failed")]
    public async Task<IActionResult> MarkAsFailed([FromBody] PaymentUpdateStatusDto dto)
    {
        await _paymentService.MarkAsFailedAsync(dto);

        return Ok(new
        {
            Message = "Payment failure recorded.",
            Timestamp = DateTime.UtcNow
        });
    }
}