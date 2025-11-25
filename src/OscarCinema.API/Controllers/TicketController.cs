using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TicketResponse>> Create([FromBody] CreateTicket dto)
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

        [Authorize]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> GetById(int id)
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

        [Authorize(Policy = "AdminOrEmployee")]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<TicketResponse>>> GetAll([FromQuery] PaginationQuery query)
        {
            _logger.LogDebug("Listing all tickets with pagination.");

            var pageResult = await _ticketService.GetAllAsync(query);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllByUserId(int userId)
        {
            _logger.LogDebug("Getting all tickets for user ID: {UserId}", userId);

            var ticketList = await _ticketService.GetAllByUserIdAsync(userId);

            _logger.LogDebug("Returning {Count} tickets for user ID: {UserId}", ticketList.Count(), userId);

            return Ok(ticketList);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllBySessionId(int sessionId)
        {
            _logger.LogDebug("Getting all tickets for session ID: {SessionId}", sessionId);

            var ticketList = await _ticketService.GetAllBySessionIdAsync(sessionId);

            _logger.LogDebug("Returning {Count} tickets for session ID: {SessionId}", ticketList.Count(), sessionId);

            return Ok(ticketList);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TicketResponse>> Update(int id, [FromBody] UpdateTicket dto)
        {
            _logger.LogInformation("Updating ticket ID: {Id} with data: {@Dto}", id, dto);

            var updatedTicket = await _ticketService.UpdateAsync(id, dto);

            _logger.LogInformation("Ticket updated successfully: {Id}", id);

            return Ok(updatedTicket);
        }

        [Authorize]
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

