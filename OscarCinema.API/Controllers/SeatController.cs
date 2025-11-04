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

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;

        }

        [HttpPost]
        public async Task<ActionResult<SeatResponseDTO>> Create([FromBody] CreateSeatDTO dto)
        {
            try
            {
                var createdSeat = await _seatService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdSeat.Id },
                    createdSeat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatResponseDTO>> GetById(int id)
        {
            var seat = await _seatService.GetByIdAsync(id);

            if (seat == null)
                return NotFound();

            return Ok(seat);
        }

        [HttpGet("rowNumber")]
        public async Task<ActionResult<IEnumerable<SeatResponseDTO>>> GetByRowAndNumber([FromBody] GetSeatByRowAndNumberDTO dto)
        {
            var seat = await _seatService.GetByRowAndNumberAsync(dto);

            return Ok(seat);
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<SeatResponseDTO>>> GetSeatsByRoomId(int roomId)
        {
            var seatList = await _seatService.GetSeatsByRoomIdAsync(roomId);

            return Ok(seatList);
        }


        [HttpPatch("{id}/occupy")]
        public async Task<ActionResult<SeatResponseDTO>> OccupySeat(int id)
        {
            try
            {
                var seat = await _seatService.OccupySeatAsync(id);
                if (seat == null)
                    return NotFound($"Seat with ID {id} not found.");

                return Ok(seat);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/free")]
        public async Task<ActionResult<SeatResponseDTO>> FreeSeat(int id)
        {
            try
            {
                var seat = await _seatService.FreeSeatAsync(id);
                if (seat == null)
                    return NotFound($"Seat with ID {id} not found.");

                return Ok(seat);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _seatService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Seat with ID {id} not found");

            return NoContent();
        }
    }
}

