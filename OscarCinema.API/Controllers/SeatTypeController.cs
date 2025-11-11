using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.Interfaces;
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

        [HttpPost]
        public async Task<ActionResult<SeatTypeResponseDTO>> Create([FromBody] CreateSeatTypeDTO dto)
        {
            _logger.LogInformation("Creating new seat type: {Name}", dto.Name);

            var createdSeatType = await _seatTypeService.CreateAsync(dto);

            _logger.LogInformation("Seat type created with ID: {Id}", createdSeatType.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdSeatType.Id },
                createdSeatType);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatTypeResponseDTO>> GetById(int id)
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeatTypeResponseDTO>>> GetAll()
        {
            _logger.LogDebug("Listing all seat types");

            var seatTypeList = await _seatTypeService.GetAllAsync();

            _logger.LogDebug("Returning {Count} seat types", seatTypeList.Count());

            return Ok(seatTypeList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SeatTypeResponseDTO>> Update(int id, [FromBody] UpdateSeatTypeDTO dto)
        {
            _logger.LogInformation("Updating seat type ID: {Id} with data: {@Dto}", id, dto);

            var updatedSeatType = await _seatTypeService.UpdateAsync(id, dto);

            _logger.LogInformation("Seat type updated successfully: {Id}", id);

            return Ok(updatedSeatType);
        }

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

