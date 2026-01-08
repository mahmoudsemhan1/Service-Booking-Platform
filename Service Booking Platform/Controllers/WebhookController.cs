using Application.Interfaces.Services.IEmailService;
using Application.Interfaces.Services.IPaymentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

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
    [AllowAnonymous]
    [HttpPost]
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