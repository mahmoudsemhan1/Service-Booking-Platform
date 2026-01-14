using Application.Common.page;
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
    /// <summary>
    /// Handles the global service catalog, allowing for service discovery, creation, and modification.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Retrieves a paged list of all available services.
        /// </summary>
        /// <param name="param">Pagination parameters (PageNumber, PageSize).</param>
        /// <returns>A paged list of services.</returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams param)
        {
            var services= await _serviceService.GetPagedAsync(param);
            return Ok(services);
        }
        /// <summary>
        /// Retrieves the details of a specific service by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the service.</param>
        /// <returns>The service details.</returns>
        /// <response code="200">Returns the requested service.</response>
        /// <response code="404">If the service does not exist.</response>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var srevice = await _serviceService.GetByIdAsync(id);
            if (srevice == null)
                return NotFound();

            return Ok(srevice); 
        }
        /// <summary>
        /// Creates a new service in the system.
        /// </summary>
        /// <remarks>
        /// This endpoint uses 'multipart/form-data' to allow image uploads.
        /// Must be authorized as a Provider.
        /// </remarks>
        /// <param name="dto">The service data including image files.</param>
        /// <returns>The created service object.</returns>
        /// <response code="201">Service created successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize(Roles = AppRoles.Provider)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
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


        /// <summary>
        /// Updates an existing service's details.
        /// </summary>
        /// <param name="id">The ID of the service to update.</param>
        /// <param name="dto">The updated service data (supports form-data for new images).</param>
        /// <response code="204">No content - Update successful.</response>
        /// <response code="400">ID mismatch or invalid data.</response>
        [Authorize(Roles = $"{AppRoles.Admin}, {AppRoles.Provider}")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// <summary>
        /// Deletes a service from the system.
        /// </summary>
        /// <param name="id">The ID of the service to delete.</param>
        /// <response code="204">No content - Deletion successful.</response>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.SuperAdmin},{AppRoles.Provider}")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
