using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Seat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly ILogger<SeatController> _logger;

        public SeatController(ISeatService seatService, ILogger<SeatController> logger)
        {
            _seatService = seatService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<SeatResponseDTO>> Create([FromBody] CreateSeatDTO dto)
        {
            _logger.LogInformation("Creating new seat - Row: {Row}, Number: {Number}, Room: {RoomId}", dto.Row, dto.Number, dto.RoomId);

            var createdSeat = await _seatService.CreateAsync(dto);

            _logger.LogInformation("Seat created with ID: {Id}", createdSeat.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdSeat.Id },
                createdSeat);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatResponseDTO>> GetById(int id)
        {
            _logger.LogDebug("Searching seat by ID: {Id}", id);

            var seat = await _seatService.GetByIdAsync(id);

            if (seat == null)
            {
                _logger.LogWarning("Seat not found: {Id}", id);
                return NotFound();
            }

            return Ok(seat);
        }

        [HttpGet("rowNumber")]
        public async Task<ActionResult<IEnumerable<SeatResponseDTO>>> GetByRowAndNumber([FromBody] GetSeatByRowAndNumberDTO dto)
        {
            _logger.LogDebug("Searching seat by row and number - Row: {Row}, Number: {Number}", dto.Row, dto.Number);

            var seat = await _seatService.GetByRowAndNumberAsync(dto);

            return Ok(seat);
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<SeatResponseDTO>>> GetSeatsByRoomId(int roomId)
        {
            _logger.LogDebug("Getting all seats for room ID: {RoomId}", roomId);

            var seatList = await _seatService.GetSeatsByRoomIdAsync(roomId);

            _logger.LogDebug("Returning {Count} seats for room ID: {RoomId}", seatList.Count(), roomId);

            return Ok(seatList);
        }

        [HttpPatch("{id}/occupy")]
        public async Task<ActionResult<SeatResponseDTO>> OccupySeat(int id)
        {
            _logger.LogInformation("Occupying seat ID: {Id}", id);

            var seat = await _seatService.OccupySeatAsync(id);
            if (seat == null)
            {
                _logger.LogWarning("Seat not found for occupation: {Id}", id);
                return NotFound($"Seat with ID {id} not found.");
            }

            _logger.LogInformation("Seat occupied successfully: {Id}", id);
            return Ok(seat);
        }

        [HttpPatch("{id}/free")]
        public async Task<ActionResult<SeatResponseDTO>> FreeSeat(int id)
        {
            _logger.LogInformation("Freeing seat ID: {Id}", id);

            var seat = await _seatService.FreeSeatAsync(id);
            if (seat == null)
            {
                _logger.LogWarning("Seat not found for freeing: {Id}", id);
                return NotFound($"Seat with ID {id} not found.");
            }

            _logger.LogInformation("Seat freed successfully: {Id}", id);
            return Ok(seat);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting seat ID: {Id}", id);

            var deleted = await _seatService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete seat not found: {Id}", id);
                return NotFound($"Seat with ID {id} not found");
            }

            _logger.LogInformation("Seat deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

