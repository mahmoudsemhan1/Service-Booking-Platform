using Application.DTOs.Service;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.IServices;
using AutoMapper.Configuration.Annotations;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services= await _serviceService.GetAllAsync();

            return Ok(services);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var srevice = await _serviceService.GetByIdAsync(id);

            return Ok(srevice); 
        }
        [Authorize(Roles = AppRoles.Provider )]
        [HttpPost]
        public async Task<IActionResult> CreateService([FromForm] ServiceCreateDto dto)
        {
               
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           if(userId == null)
            {
                return Unauthorized();
            }   

            var service = await _serviceService.CreateAsync(dto,userId);
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }


        [Authorize(Roles = $"{AppRoles.Admin}, {AppRoles.Provider}" )]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int  id,[FromForm] ServiceUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch between URL and body.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId==null)
                return Unauthorized();

            var service= await _serviceService.UpdateAsync(id,dto,userId);

            return NoContent();
        }
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.SuperAdmin},{AppRoles.Provider}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteservice(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            var service= await _serviceService.DeleteAsync(id,userId);

            return NoContent();
        }


    }
}
