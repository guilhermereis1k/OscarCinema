using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatTypeController : ControllerBase
    {
        private readonly ISeatTypeService _seatTypeService;
        private readonly ILogger<SeatTypeController> _logger;

        public SeatTypeController(ISeatTypeService seatTypeService, ILogger<SeatTypeController> logger)
        {
            _seatTypeService = seatTypeService;
            _logger = logger;
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpPost]
        public async Task<ActionResult<SeatTypeResponse>> Create([FromBody] CreateSeatType dto)
        {
            _logger.LogInformation("Creating new seat type: {Name}", dto.Name);

            var createdSeatType = await _seatTypeService.CreateAsync(dto);

            _logger.LogInformation("Seat type created with ID: {Id}", createdSeatType.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdSeatType.Id },
                createdSeatType);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<SeatTypeResponse>> GetById(int id)
        {
            _logger.LogDebug("Searching seat type by ID: {Id}", id);

            var seatType = await _seatTypeService.GetByIdAsync(id);

            if (seatType == null)
            {
                _logger.LogWarning("Seat type not found: {Id}", id);
                return NotFound();
            }

            return Ok(seatType);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<SeatTypeResponse>>> GetAll([FromQuery] PaginationQuery query)
        {
            _logger.LogDebug("Listing all seat types with pagination.");

            var pageResult = await _seatTypeService.GetAllAsync(query);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SeatTypeResponse>> Update(int id, [FromBody] UpdateSeatType dto)
        {
            _logger.LogInformation("Updating seat type ID: {Id} with data: {@Dto}", id, dto);

            var updatedSeatType = await _seatTypeService.UpdateAsync(id, dto);

            _logger.LogInformation("Seat type updated successfully: {Id}", id);

            return Ok(updatedSeatType);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting seat type: {Id}", id);

            var deleted = await _seatTypeService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete seat type not found: {Id}", id);
                return NotFound($"SeatType with ID {id} not found");
            }

            _logger.LogInformation("Seat type deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

