using Application.DTOs.Payment;
using Application.Interfaces.Services.IPaymentService;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
/// <summary>
/// Handles payment processing, including Stripe checkout sessions, payment history, and refunds.
/// </summary>

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;

    /// <summary>
    /// Retrieves a complete list of all payments in the system.
    /// </summary>
    /// <remarks>Accessible only by users with the Admin role.</remarks>
    /// <returns>A list of all payments.</returns>
    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllPayments()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    /// <summary>
    /// Retrieves details of a specific payment by its ID.
    /// </summary>
    /// <param name="paymentId">The unique ID of the payment record.</param>
    /// <returns>The payment details.</returns>
    /// <response code="200">Returns the requested payment details.</response>
    /// <response code="404">If the payment record is not found.</response>
    [HttpGet("{paymentId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(int paymentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var payment = await _paymentService.GetByIdAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

        return Ok(payment);
    }

    /// <summary>
    /// Records a manual payment entry in the system.
    /// </summary>
    /// <param name="dto">The payment creation data.</param>
    /// <returns>The created payment record.</returns>
    [HttpPost("checkout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var payment = await _paymentService.CreateAsync(dto, userId!);

        return CreatedAtAction(nameof(GetPaymentById), new { paymentId = payment.Id }, payment);
    }

    /// <summary>
    /// Processes a refund for a previously captured payment.
    /// </summary>
    /// <remarks>Admin-only operation. Interacts with the payment gateway to return funds.</remarks>
    /// <param name="paymentId">The ID of the payment to be refunded.</param>
    [HttpPost("{paymentId}/refund")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefundPayment(int paymentId)
    {
        await _paymentService.RefundAsync(paymentId);
        return Ok(new { Message = "Refund processed successfully." });
    }

    /// <summary>
    /// Generates a Stripe Checkout Session URL for a specific booking.
    /// </summary>
    /// <remarks>
    /// This is the primary method for online payments. 
    /// The frontend should redirect the user to the returned URL to complete the payment on Stripe's secure page.
    /// </remarks>
    /// <param name="bookingId">The ID of the booking to pay for.</param>
    /// <returns>A JSON object containing the Stripe Session URL.</returns>
    /// <response code="200">Returns the URL for the checkout page.</response>
    [HttpPost("create-checkout-session/{bookingId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCheckoutSession(int bookingId)
    {
        var userid =  User.FindFirstValue(ClaimTypes.NameIdentifier);
        // 1. نادي ميثود السيرفيس اللي بتكلم Stripe
        // تأكد إن IPaymentService فيها ميثود بترجع الـ URL
        var sessionUrl = await _paymentService.CreateCheckoutSessionAsync(bookingId, userid);

        // 2. رجع الـ URL عشان الـ Frontend (أو Postman) يفتحه
        return Ok(new { url = sessionUrl });
    }
}