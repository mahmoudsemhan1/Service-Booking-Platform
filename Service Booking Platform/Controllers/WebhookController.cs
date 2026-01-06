using Application.Interfaces.Services.IPaymentService;
using Microsoft.AspNetCore.Mvc;
using Stripe;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _config;

    public WebhookController(IPaymentService paymentService, IConfiguration config)
    {
        _paymentService = paymentService;
        _config = config;
    }

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
                
                if (stripeEvent.Data.Object is Stripe.Checkout.Session session)
                {
                    await _paymentService.HandlePaymentSuccessAsync(session.Id);
                }
            }

            return Ok(); 
        }
        catch (StripeException)
        {
            return BadRequest(); 
        }
    }
}