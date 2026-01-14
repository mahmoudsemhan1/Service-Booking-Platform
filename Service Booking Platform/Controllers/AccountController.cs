using Application.Common.page;
using Application.DTOs.Account;
using Application.Interfaces.Services.IUserIdentityServices;
using Application.Interfaces.Services.TokenService;
using Domain.Constants;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using static Application.DTOs.Account.Account;

namespace Service_Booking_Platform.Controllers
{
    /// <summary>
    /// Manages user accounts, including registration, login, and external authentication (Google).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitofWork _unitofWork;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserIdentityService _identityService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IUnitofWork unitofWork,
            SignInManager<ApplicationUser> signInManager
,
            IUserIdentityService identityService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitofWork = unitofWork;
            _signInManager = signInManager;
            _identityService = identityService;
        }
        /// <summary>
        /// Retrieves a paged list of all registered users.
        /// </summary>
        /// <param name="paging">Pagination parameters (PageNumber and PageSize).</param>
        /// <returns>A paged list of users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="403">If the user is not a SuperAdmin.</response>
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParams paging)
        {
            var result = await _identityService.GetAllUsersPagedAsync(paging);

            if (result.TotalCount == 0)
                return Ok(new { message = "No users found", data = result });

            return Ok(result);
        }
        /// <summary>
        /// Registers a new user or provider.
        /// </summary>
        /// <param name="dto">Registration details including role (User or Provider).</param>
        /// <returns>Authentication token and success message.</returns>
        /// <response code="200">Successful registration.</response>
        /// <response code="400">If validation fails or role is invalid.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {

            // change role format to have first letter uppercase and rest lowercase (e.g., "user" -> "User")
            var requestedRole = AppRoles.RegistrationRoles
                                        .FirstOrDefault(r => r.Equals(dto.Role, StringComparison.OrdinalIgnoreCase));
            if (requestedRole == null)
                return BadRequest("Invalid Role. You can only register as User or Provider.");

            await _unitofWork.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = true

                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    await _unitofWork.RollbackTransactionAsync();
                    return BadRequest(result.Errors);
                }


                await _userManager.AddToRoleAsync(user, requestedRole);

                var userProfile = new UserProfile
                    (
                         user.Id,
                         dto.bio ?? "Welcome to our platform!"
                    );
                    
                         await _unitofWork.UserProfiles.AddAsync(userProfile);

                if (requestedRole == AppRoles.Provider)
                {
                    await _unitofWork.Providers.AddAsync(new Provider(user.Id, dto.FullName));
                }

                await _unitofWork.CommitTransactionAsync();

                //  توليد التوكن للرد السريع
                var roles = new List<string> { requestedRole };
                var token = _tokenService.CreateToken(user.Id, user.Email!, user.FullName, roles);

                return Ok(new AuthResponseDto(
                    Token: token,
                    ExpiresOn: DateTime.UtcNow.AddDays(7),
                    Message: "User registered successfully",
                    IsSuccess: true
                ));
            }
            catch (Exception)
            {
                // in case of any error, rollback the transaction
                await _unitofWork.RollbackTransactionAsync();
                throw;
            }
        }
        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">Login credentials.</param>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            var existingUser = await _userManager.FindByEmailAsync(loginDto.Email);
            if (existingUser == null || !await _userManager.CheckPasswordAsync(existingUser, loginDto.Password))
                return Unauthorized("Invalid login attempt.");
            var Roles = await _userManager.GetRolesAsync(existingUser);
            var token = _tokenService.CreateToken(existingUser.Id, existingUser.Email!, existingUser.FullName!, Roles);

            return Ok(new AuthResponseDto(token, DateTime.UtcNow.AddDays(7), "Done", true));


        }
        /// <summary>
        /// Administrative tool to create a user with any role.
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPost("create-user")]
     
        public async Task<IActionResult> SuperCreateUser([FromBody] RegisterDto dto)
        {

            var allRoles = new List<string> { AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.User, AppRoles.Provider };

            var requestedRole = allRoles
                .FirstOrDefault(r => r.Equals(dto.Role, StringComparison.OrdinalIgnoreCase));

            if (requestedRole == null)
            {
                return BadRequest(new { message = $"Role '{dto.Role}' does not exist." });
            }

            await _unitofWork.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    await _unitofWork.RollbackTransactionAsync();
                    return BadRequest(result.Errors);
                }

                await _userManager.AddToRoleAsync(user, requestedRole);

                var userProfile = new UserProfile ( user.Id,  dto.bio ?? "Created by Admin" );
                await _unitofWork.UserProfiles.AddAsync(userProfile);

                if (requestedRole == AppRoles.Provider)
                {
                    await _unitofWork.Providers.AddAsync(new Provider(user.Id, dto.FullName));
                }

                await _unitofWork.CommitTransactionAsync();

                return Ok(new { message = $"Account with role '{requestedRole}' created successfully." });
            }
            catch (Exception)
            {
                await _unitofWork.RollbackTransactionAsync();
                throw;
            }
        }
        /// <summary>
        /// Gets the profile information of the currently authenticated user.
        /// </summary>
        [Authorize]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                Roles = roles
            });
        }
        /// <summary>
        /// Changes the password for the logged-in user.
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "Password changed successfully." });
        }
        /// <summary>
        /// Deletes a user and all associated data (Profile, Provider info).
        /// </summary>
        /// <param name="userId">The unique identifier of the user to delete.</param>
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            await _unitofWork.BeginTransactionAsync();
            try
            {
                // Delete  it from domain 
                var profile = (await _unitofWork.UserProfiles.FindAsync(p => p.UserId == userId)).FirstOrDefault();
                if (profile != null) await _unitofWork.UserProfiles.DeleteAsync(profile);
                //delete provider if exists 
                var provider = (await _unitofWork.Providers.FindAsync(p => p.UserId == userId)).FirstOrDefault();
                if (provider != null) await _unitofWork.Providers.DeleteAsync(provider);
                // delete from identity
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    await _unitofWork.RollbackTransactionAsync();
                    return BadRequest(result.Errors);
                }
                await _unitofWork.CommitTransactionAsync();
                return Ok(new { message = "User deleted successfully." });

            }
            catch (Exception)
            {
                await _unitofWork.RollbackTransactionAsync();
                throw;
            }
        }
        // Google Authentication Endpoints

        // 1. الميثود اللي بتبدأ الطلب
        /// <summary>
        /// Redirects the user to Google for external authentication.
        /// </summary>
        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            // بنحدد المسار اللي جوجل هيرجع عليه بعد ما اليوزر يوافق
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        // 2. الميثود اللي بتستقبل بيانات جوجل'
        /// <summary>
        /// Callback endpoint for Google Authentication.
        /// </summary>
        /// <remarks>
        /// If the user is new, an account is automatically created with the "User" role.
        /// </remarks>
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return BadRequest("Error loading external login information.");

            // محاولة تسجيل الدخول لو اليوزر مسجل بجوجل قبل كدة
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var roles = await _userManager.GetRolesAsync(user!);

                // استخدمنا هنا FullName لضمان الـ Consistency مع الـ Register والـ Login العادي
                var token = _tokenService.CreateToken(user!.Id, user.Email!, user.FullName, roles);

                return Ok(new AuthResponseDto(token, DateTime.UtcNow.AddDays(7), "Welcome back!", true));
            }

            // --- حالة مستخدم جديد (تسجيل لأول مرة بجوجل) ---

            await _unitofWork.BeginTransactionAsync();

            try
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name,
                    EmailConfirmed = true // بما إنه جاي من جوجل فالايميل موثق
                };

                var createResult = await _userManager.CreateAsync(newUser);

                if (!createResult.Succeeded)
                {
                    await _unitofWork.RollbackTransactionAsync();
                    return BadRequest(createResult.Errors);
                }

                // 1. ربط الحساب بجوجل في جداول الـ Identity
                await _userManager.AddLoginAsync(newUser, info);

                // 2. إعطاؤه دور "User" كافتراضي
                await _userManager.AddToRoleAsync(newUser, "User");

                // 3. إنشاء الـ UserProfile (التعديل اللي كان ناقص)
                var userProfile = new UserProfile
                (
                    newUser.Id,
                     "Signed up via Google"
                );
                await _unitofWork.UserProfiles.AddAsync(userProfile);

                // تنفيذ كل العمليات في قاعدة البيانات
                await _unitofWork.CommitTransactionAsync();

                var token = _tokenService.CreateToken(newUser.Id, newUser.Email!, newUser.FullName, new List<string> { "User" });

                return Ok(new AuthResponseDto(
                    Token: token,
                    ExpiresOn: DateTime.UtcNow.AddDays(7),
                    Message: "Registration successful via Google",
                    IsSuccess: true
                ));
            }
            catch (Exception)
            {
                await _unitofWork.RollbackTransactionAsync();
                throw;
            }
        }





    }
}
