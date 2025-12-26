using Application.DTOs.Service;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.IServices;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service_Booking_Platform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciesController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IFileService _fileService;

        public ServiciesController(IServiceService serviceService, IFileService fileService)
        {
            _serviceService = serviceService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services= await _serviceService.GetAllAsync();

            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var srevice = await _serviceService.GetByIdAsync(id);

            return Ok(srevice); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateSrvice([FromForm] ServiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var service = await _serviceService.CreateAsync(dto);
            return Ok(service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int  id,[FromForm] ServiceUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch between URL and body.");
            }
            var service= await _serviceService.UpdateAsync(id, dto);

            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Deleteservice(int id)
        {
            var service= await _serviceService.DeleteAsync(id);

            return Ok();
        }


    }
}
