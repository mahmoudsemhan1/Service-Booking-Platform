using Application.DTOs.providerServiceDto;
using Application.Interfaces.Services.IProviderSerivce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Service_Booking_Platform.Controllers
{

    // <summary>
    /// Manages provider-specific operations, including profile management and service assignments.
    /// </summary>
    /// <remarks>
    /// All endpoints in this controller require the user to have the 'Provider' role.
    /// </remarks>
    [Authorize(Roles = "Provider")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderAppService _providerService;

        public ProviderController(IProviderAppService providerAppService)
        {
            _providerService = providerAppService;
        }
        /// <summary>
        /// Retrieves the business profile of the authenticated provider.
        /// </summary>
        /// <returns>Provider business details and assigned services.</returns>
        /// <response code="200">Returns the provider profile.</response>
        /// <response code="401">If the user is not authenticated or not a provider.</response>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var profile = await _providerService.GetProfileAsync(userId);
            return Ok(profile);
        }

        /// <summary>
        /// Updates the provider's business information (e.g., Business Name, Address).
        /// </summary>
        /// <param name="dto">The updated business info details.</param>
        /// <response code="204">Profile updated successfully.</response>
        /// <response code="400">If the input data is invalid.</response>
        [HttpPut("business-info")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBusinessInfo([FromBody] UpdateBusinessInfoDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            await _providerService.UpdateBusinessInfoAsync(userId, dto);
            return NoContent();
        }
        /// <summary>
        /// Assigns one or more global services to the provider's catalog with custom pricing.
        /// </summary>
        /// <param name="dto">A list of service IDs and their corresponding prices.</param>
        /// <response code="200">Services assigned successfully.</response>
        [HttpPost("assign-services")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignServices([FromBody] AssignServicesDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            await _providerService.AssignServicesAsync(userId, dto);
            return Ok(new { message = "Service assign successfully" });
        }
        /// <summary>
        /// Removes a service from the provider's personal catalog.
        /// </summary>
        /// <param name="serviceId">The ID of the service to remove.</param>
        /// <response code="200">Service removed successfully.</response>
        /// <response code="404">If the service link is not found for this provider.</response>
        [HttpDelete("remove-service/{serviceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveService(int serviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            await _providerService.RemoveServiceAsync(userId, serviceId);
            return Ok(new { message = "Service removed successfully" });

        }

    }
}
