using Application.DTOs.providerServiceDto;
using Application.Interfaces.Services.IProviderSerivce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Service_Booking_Platform.Controllers
{
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
        [HttpGet("profile")]
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

        [HttpPut("business-info")]
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
        [HttpPost("assign-services")]
        public async Task<IActionResult> AssignServices([FromBody] AssignServicesDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            await _providerService.AssignServicesAsync(userId, dto);
            return Ok(new {message="Service assign successfully"});
        }
        [HttpDelete("remove-service/{serviceId}")]
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
