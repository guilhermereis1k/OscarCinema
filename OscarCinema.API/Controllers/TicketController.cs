using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ITicketService ticketService, ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponseDTO>> Create([FromBody] CreateTicketDTO dto)
        {
            _logger.LogInformation("Creating new ticket for session {SessionId}, with {SeatCount} seats.",
                dto.SessionId, dto.TicketSeats.Count);

            var createdTicket = await _ticketService.CreateAsync(dto);

            _logger.LogInformation("Ticket created with ID: {Id}", createdTicket.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTicket.Id },
                createdTicket);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponseDTO>> GetById(int id)
        {
            _logger.LogDebug("Searching ticket by ID: {Id}", id);

            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found: {Id}", id);
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAll()
        {
            _logger.LogDebug("Listing all tickets");

            var ticketList = await _ticketService.GetAllAsync();

            _logger.LogDebug("Returning {Count} tickets", ticketList.Count());

            return Ok(ticketList);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAllByUserId(int userId)
        {
            _logger.LogDebug("Getting all tickets for user ID: {UserId}", userId);

            var ticketList = await _ticketService.GetAllByUserIdAsync(userId);

            _logger.LogDebug("Returning {Count} tickets for user ID: {UserId}", ticketList.Count(), userId);

            return Ok(ticketList);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAllBySessionId(int sessionId)
        {
            _logger.LogDebug("Getting all tickets for session ID: {SessionId}", sessionId);

            var ticketList = await _ticketService.GetAllBySessionIdAsync(sessionId);

            _logger.LogDebug("Returning {Count} tickets for session ID: {SessionId}", ticketList.Count(), sessionId);

            return Ok(ticketList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TicketResponseDTO>> Update(int id, [FromBody] UpdateTicketDTO dto)
        {
            _logger.LogInformation("Updating ticket ID: {Id} with data: {@Dto}", id, dto);

            var updatedTicket = await _ticketService.UpdateAsync(id, dto);

            _logger.LogInformation("Ticket updated successfully: {Id}", id);

            return Ok(updatedTicket);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting ticket: {Id}", id);

            var deleted = await _ticketService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete ticket not found: {Id}", id);
                return NotFound($"Ticket with ID {id} not found");
            }

            _logger.LogInformation("Ticket deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

