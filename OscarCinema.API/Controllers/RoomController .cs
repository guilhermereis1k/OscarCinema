using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

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

        [HttpPost]
        public async Task<ActionResult<RoomResponseDTO>> Create([FromBody] CreateRoomDTO dto)
        {
            _logger.LogInformation("Creating new room: {RoomNumber}", dto.Number);

            var createdRoom = await _roomService.CreateAsync(dto);

            _logger.LogInformation("Room created with ID: {Id}", createdRoom.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdRoom.Id },
                createdRoom);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomResponseDTO>> GetById(int id)
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomResponseDTO>>> GetAll()
        {
            _logger.LogDebug("Listing all rooms");

            var roomList = await _roomService.GetAllAsync();

            _logger.LogDebug("Returning {Count} rooms", roomList.Count());

            return Ok(roomList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RoomResponseDTO>> Update(int id, [FromBody] UpdateRoomDTO dto)
        {
            _logger.LogInformation("Updating room ID: {Id} with data: {@Dto}", id, dto);

            var updatedRoom = await _roomService.UpdateAsync(id, dto);

            _logger.LogInformation("Room updated successfully: {Id}", id);

            return Ok(updatedRoom);
        }

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

        [HttpGet("number/{number}")]
        public async Task<ActionResult<RoomResponseDTO>> GetByNumber(int number)
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

        [HttpPost("addSeats/{roomId}")]
        public async Task<ActionResult<RoomResponseDTO>> AddSeats(int roomId, AddSeatsToRoomDTO dto)
        {
            _logger.LogInformation("Adding {SeatCount} seats to room ID: {RoomId}", dto.Seats.Count, roomId);

            var updatedRoom = await _roomService.AddSeatsAsync(roomId, dto);

            _logger.LogInformation("Seats added successfully to room ID: {RoomId}", roomId);

            return Ok(updatedRoom);
        }
    }
}

