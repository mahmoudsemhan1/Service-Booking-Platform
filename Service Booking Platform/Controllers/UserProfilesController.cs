using Application.DTOs.UserProfile;
using Application.Interfaces.Services.IUserProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service_Booking_Platform.Controllers
{
    /// <summary>
    /// Manages personal user profile information, including bio and profile pictures.
    /// </summary>
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

        /// <summary>
        /// Retrieves the profile details of the currently authenticated user.
        /// </summary>
        /// <returns>The user's profile data.</returns>
        /// <response code="200">Returns the user profile.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("Me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
                return Unauthorized();

            var profile = await _userProfileService.GetByUserIdAsync(userId);
            return Ok(profile);
        }

        /// <summary>
        /// Updates the authenticated user's profile information.
        /// </summary>
        /// <remarks>
        /// This endpoint accepts 'multipart/form-data'. 
        /// Use this to update the bio or upload a new profile image.
        /// </remarks>
        /// <param name="userProfileDto">The profile update data (Bio, ProfileImage, etc.).</param>
        /// <returns>The updated profile record.</returns>
        /// <response code="200">Returns the updated profile.</response>
        /// <response code="400">If the data provided is invalid.</response>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

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
