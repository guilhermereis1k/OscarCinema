using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpPost]
        public async Task<ActionResult<RoomResponse>> Create([FromBody] CreateRoom dto)
        {
            _logger.LogInformation("Creating new room: {RoomNumber}", dto.Number);

            var createdRoom = await _roomService.CreateAsync(dto);

            _logger.LogInformation("Room created with ID: {Id}", createdRoom.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdRoom.Id },
                createdRoom);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomResponse>> GetById(int id)
        {
            _logger.LogDebug("Searching room by ID: {Id}", id);

            var room = await _roomService.GetByIdAsync(id);

            if (room == null)
            {
                _logger.LogWarning("Room not found: {Id}", id);
                return NotFound();
            }

            return Ok(room);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<RoomResponse>>> GetAll([FromQuery] PaginationQuery query)
        {
            _logger.LogDebug("Listing all rooms with pagination.");

            var pageResult = await _roomService.GetAllAsync(query);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RoomResponse>> Update(int id, [FromBody] UpdateRoom dto)
        {
            _logger.LogInformation("Updating room ID: {Id} with data: {@Dto}", id, dto);

            var updatedRoom = await _roomService.UpdateAsync(id, dto);

            _logger.LogInformation("Room updated successfully: {Id}", id);

            return Ok(updatedRoom);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting room: {Id}", id);

            var deleted = await _roomService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete room not found: {Id}", id);
                return NotFound($"Room with ID {id} not found");
            }

            _logger.LogInformation("Room deleted successfully: {Id}", id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("number/{number}")]
        public async Task<ActionResult<RoomResponse>> GetByNumber(int number)
        {
            _logger.LogDebug("Searching room by number: {Number}", number);

            var room = await _roomService.GetByNumberAsync(number);

            if (room == null)
            {
                _logger.LogWarning("Room not found with number: {Number}", number);
                return NotFound();
            }

            return Ok(room);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpPost("addSeats/{roomId}")]
        public async Task<ActionResult<RoomResponse>> AddSeats(int roomId, AddSeatsToRoom dto)
        {
            _logger.LogInformation("Adding {SeatCount} seats to room ID: {RoomId}", dto.Seats.Count, roomId);

            var updatedRoom = await _roomService.AddSeatsAsync(roomId, dto);

            _logger.LogInformation("Seats added successfully to room ID: {RoomId}", roomId);

            return Ok(updatedRoom);
        }
    }
}

