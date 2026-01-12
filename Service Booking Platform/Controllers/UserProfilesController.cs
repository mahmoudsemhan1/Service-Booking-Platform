using Application.DTOs.UserProfile;
using Application.Interfaces.Services.IUserProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service_Booking_Platform.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfilesController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }


        [HttpGet("Me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
                return Unauthorized();

            var profile = await _userProfileService.GetByUserIdAsync(userId);
            return Ok(profile);
        }

        [HttpPut("update")]

        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto   userProfileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
                return Unauthorized();

            var updatedProfile = await _userProfileService.UpdateAsync(userId, userProfileDto);
            return Ok(updatedProfile);
        }

    }
}
