using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.TicketSeat;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketSeatController : ControllerBase
    {
        private readonly ITicketSeatService _ticketSeatService;

        public TicketSeatController(ITicketSeatService ticketSeatService)
        {
            _ticketSeatService = ticketSeatService;

        }


        [HttpPost]
        public async Task<ActionResult<TicketSeatResponseDTO>> Create([FromBody] CreateTicketSeatDTO dto)
        {
            try
            {
                var createdTicketSeat = await _ticketSeatService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdTicketSeat.Id },
                    createdTicketSeat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("multiple")]
        public async Task<ActionResult<TicketSeatResponseDTO>> CreateMultiple([FromBody] IEnumerable<CreateTicketSeatDTO> dto)
        {
            try
            {
                var createdTicketSeat = await _ticketSeatService.CreateMultipleAsync(dto);

                return Ok(createdTicketSeat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketSeatResponseDTO>> GetById(int id)
        {
            var ticketSeat = await _ticketSeatService.GetByIdAsync(id);

            if (ticketSeat == null)
                return NotFound();

            return Ok(ticketSeat);
        }

        [HttpGet("ticket/{id}")]
        public async Task<ActionResult<IEnumerable<TicketSeatResponseDTO>>> GetByTicketId(int id)
        {
            var ticketSeat = await _ticketSeatService.GetByTicketIdAsync(id);

            if (ticketSeat == null)
                return NotFound();

            return Ok(ticketSeat);
        }

        [HttpGet("seat/{id}")]
        public async Task<ActionResult<IEnumerable<TicketSeatResponseDTO>>> GetBySeatId(int id)
        {
            var ticketSeat = await _ticketSeatService.GetBySeatIdAsync(id);

            if (ticketSeat == null)
                return NotFound();

            return Ok(ticketSeat);
        }

        [HttpPut("price/{id:int}")]
        public async Task<ActionResult<TicketSeatResponseDTO>> UpdatePriceAsync(int id, [FromBody] decimal newPrice)
        {
            try
            {
                var updatedTicketSeat = await _ticketSeatService.UpdatePriceAsync(id, newPrice);
                return Ok(updatedTicketSeat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("calculate/{id:int}")]
        public async Task<ActionResult<decimal>> CalculateTicketTotal(int id)
        {
            try
            {
                var newValue = await _ticketSeatService.CalculateTicketTotalAsync(id);

                return Ok(newValue);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _ticketSeatService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"TicketSeat with ID {id} not found");

            return NoContent();
        }
    }
}

