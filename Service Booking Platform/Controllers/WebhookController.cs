using Application.Interfaces.Services.IEmailService;
using Application.Interfaces.Services.IPaymentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
/// <summary>
/// Handles incoming HTTP callbacks (Webhooks) from Stripe to process background events.
/// </summary>
/// <remarks>
/// This controller does not require authentication because it is called externally by Stripe. 
/// Security is maintained by verifying the 'Stripe-Signature' header using the Webhook Secret.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public WebhookController(IPaymentService paymentService, IConfiguration config, IEmailService emailService)
    {
        _paymentService = paymentService;
        _config = config;
        _emailService = emailService;
    }
    /// <summary>
    /// Receives and processes Stripe events (e.g., checkout.session.completed).
    /// </summary>
    /// <remarks>
    /// When a payment is successful on Stripe's hosted page, Stripe sends a POST request here.
    /// We verify the event, update the booking status in our database, and send a confirmation email.
    /// </remarks>
    /// <returns>A 200 OK status to acknowledge receipt of the event.</returns>
    /// <response code="200">Event received and processed.</response>
    /// <response code="400">If the signature verification fails or the request is invalid.</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index()
    {
        // 1. read the request body from Stripe
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            // 2. check the event's signature to verify it's from Stripe 
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _config["Stripe:WebhookSecret"]
            );

            // 3.  filter the event type we care about  , like payment success  
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                if (session != null)
                {
                    // 
                    await _paymentService.HandlePaymentSuccessAsync(session.Id);

                    // git the customer email and booking details from metadata
                    var userEmail = session.CustomerDetails.Email; 
                    var bookingId = session.Metadata.ContainsKey("BookingId") ? session.Metadata["BookingId"] : "Unknown";

                    // send confirmation email to the user 
                    string emailBody = $@"
                        <div style='font-family: Arial; direction: rtl; text-align: right;'>
                            <h2 style='color: #2d89ef;'>Payment has been confirmed successfully! ✅</h2>
                            <p>Dear customer, your booking number has been confirmed <strong>#{bookingId}</strong> Successfully.</p>
                            <p> Total amount paid: <strong>{session.AmountTotal / 100.0} {session.Currency.ToUpper()}</strong></p>
                            <br/>
                            <p>Thank you for using our platform.</p>
                        </div>";

                    await _emailService.SendEmailAsync(userEmail, "Confirm payment and booking", emailBody);
                }
            }

            return Ok();
        }
        catch (StripeException  ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}