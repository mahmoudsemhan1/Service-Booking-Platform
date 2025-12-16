//using Application.DTOs.Identity;
//using Application.DTOs.User;
//using Application.Interfaces.Services.IUserService;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;

//namespace Service_Booking_Platform.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly IUSerService _userService;

//        public UserController(IUSerService userService)
//        {
//            _userService = userService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var users = await _userService.GetAllAsync();
//            return Ok(users);
//        }
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(string userid)
//        {
//            var user= await _userService.GetByIdAsync(userid);
//            return Ok(user);
//        }
//        [HttpPost]
//        public async Task<IActionResult> CreateUSer([FromBody] UserCreateDto dto)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);

//            var user =await _userService.CreateAsync(dto);
//            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
//        }
//        [HttpPut("{UserId}")]
//        public async Task<IActionResult> UpdateUser(string UserId, [FromBody] AppUserUpdateDto dto)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);
//            var user=await _userService.UpdateAsync(UserId, dto);

//            return Ok(dto);

//        }
//        [HttpDelete("{UserId}")]
//        public async Task<IActionResult> DeleteUser(string userid)
//        {
//            var user= await _userService.DeleteAsync(userid);

//            return NoContent();

//        }



//    }
//}
