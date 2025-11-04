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

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;

        }

        [HttpPost]
        public async Task<ActionResult<RoomResponseDTO>> Create([FromBody] CreateRoomDTO dto)
        {
            try
            {
                var createdRoom = await _roomService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdRoom.Id },
                    createdRoom);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomResponseDTO>> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);

            if (room == null)
                return NotFound();

            return Ok(room);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomResponseDTO>>> GetAll()
        {
            var roomList = await _roomService.GetAllAsync();

            return Ok(roomList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RoomResponseDTO>> Update(int id, [FromBody] UpdateRoomDTO dto)
        {
            try
            {
                var updatedRoom = await _roomService.UpdateAsync(id, dto);
                return Ok(updatedRoom);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _roomService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Room with ID {id} not found");

            return NoContent();
        }

        [HttpGet("number/{number}")]
        public async Task<ActionResult<RoomResponseDTO>> GetByNumber(int number)
        {
            var room = await _roomService.GetByNumberAsync(number);

            if (room == null)
                return NotFound();

            return Ok(room);
        }

        [HttpPost("addSeats/{roomId}")]
        public async Task<ActionResult<RoomResponseDTO>> AddSeats(int roomId, AddSeatsToRoomDTO dto)
        {
            try
            {
                var updatedRoom = await _roomService.AddSeatsAsync(roomId, dto);

                return Ok(updatedRoom);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

