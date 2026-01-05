using Application.DTOs.Payment;
using Application.Interfaces.Services.IPaymentService;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllPayments()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    [HttpGet("{paymentId}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentById(int paymentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var payment = await _paymentService.GetByIdAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

        return Ok(payment);
    }

    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var payment = await _paymentService.CreateAsync(dto, userId!);

        return CreatedAtAction(nameof(GetPaymentById), new { paymentId = payment.Id }, payment);
    }

    [HttpPost("{paymentId}/refund")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> RefundPayment(int paymentId)
    {
        await _paymentService.RefundAsync(paymentId);
        return Ok(new { Message = "Refund processed successfully." });
    }
}