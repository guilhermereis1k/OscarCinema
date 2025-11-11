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
        private readonly ILogger<TicketSeatController> _logger;

        public TicketSeatController(ITicketSeatService ticketSeatService, ILogger<TicketSeatController> logger)
        {
            _ticketSeatService = ticketSeatService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TicketSeatResponseDTO>> Create([FromBody] CreateTicketSeatDTO dto)
        {
            _logger.LogInformation("Creating ticket seat for ticket {TicketId} and seat {SeatId}",
                dto.TicketId, dto.SeatId);

            var createdTicketSeat = await _ticketSeatService.CreateAsync(dto);

            _logger.LogInformation("Ticket seat created with ID: {Id}", createdTicketSeat.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTicketSeat.Id },
                createdTicketSeat);
        }

        [HttpPost("multiple")]
        public async Task<ActionResult<TicketSeatResponseDTO>> CreateMultiple([FromBody] IEnumerable<CreateTicketSeatDTO> dto)
        {
            _logger.LogInformation("Creating multiple ticket seats. Count: {Count}", dto.Count());

            var createdTicketSeat = await _ticketSeatService.CreateMultipleAsync(dto);

            _logger.LogInformation("Multiple ticket seats created successfully. Total created: {Count}",
                createdTicketSeat.Count());

            return Ok(createdTicketSeat);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketSeatResponseDTO>> GetById(int id)
        {
            _logger.LogDebug("Searching ticket seat by ID: {Id}", id);

            var ticketSeat = await _ticketSeatService.GetByIdAsync(id);

            if (ticketSeat == null)
            {
                _logger.LogWarning("Ticket seat not found: {Id}", id);
                return NotFound();
            }

            return Ok(ticketSeat);
        }

        [HttpGet("ticket/{id}")]
        public async Task<ActionResult<IEnumerable<TicketSeatResponseDTO>>> GetByTicketId(int id)
        {
            _logger.LogDebug("Getting ticket seats for ticket ID: {TicketId}", id);

            var ticketSeat = await _ticketSeatService.GetByTicketIdAsync(id);

            if (ticketSeat == null)
            {
                _logger.LogWarning("No ticket seats found for ticket ID: {TicketId}", id);
                return NotFound();
            }

            _logger.LogDebug("Returning {Count} ticket seats for ticket ID: {TicketId}", ticketSeat.Count(), id);
            return Ok(ticketSeat);
        }

        [HttpGet("seat/{id}")]
        public async Task<ActionResult<IEnumerable<TicketSeatResponseDTO>>> GetBySeatId(int id)
        {
            _logger.LogDebug("Getting ticket seats for seat ID: {SeatId}", id);

            var ticketSeat = await _ticketSeatService.GetBySeatIdAsync(id);

            if (ticketSeat == null)
            {
                _logger.LogWarning("No ticket seats found for seat ID: {SeatId}", id);
                return NotFound();
            }

            _logger.LogDebug("Returning {Count} ticket seats for seat ID: {SeatId}", ticketSeat.Count(), id);
            return Ok(ticketSeat);
        }

        [HttpPut("price/{id:int}")]
        public async Task<ActionResult<TicketSeatResponseDTO>> UpdatePriceAsync(int id, [FromBody] decimal newPrice)
        {
            _logger.LogInformation("Updating price for ticket seat ID: {Id} to {NewPrice}", id, newPrice);

            var updatedTicketSeat = await _ticketSeatService.UpdatePriceAsync(id, newPrice);

            _logger.LogInformation("Ticket seat price updated successfully: {Id}", id);

            return Ok(updatedTicketSeat);
        }

        [HttpPut("calculate/{id:int}")]
        public async Task<ActionResult<decimal>> CalculateTicketTotal(int id)
        {
            _logger.LogInformation("Calculating total for ticket ID: {TicketId}", id);

            var newValue = await _ticketSeatService.CalculateTicketTotalAsync(id);

            _logger.LogInformation("Ticket total calculated: {Total} for ticket ID: {TicketId}", newValue, id);

            return Ok(newValue);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting ticket seat: {Id}", id);

            var deleted = await _ticketSeatService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete ticket seat not found: {Id}", id);
                return NotFound($"TicketSeat with ID {id} not found");
            }

            _logger.LogInformation("Ticket seat deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

