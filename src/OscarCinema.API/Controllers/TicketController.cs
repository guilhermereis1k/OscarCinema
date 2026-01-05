using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Room;
using OscarCinema.Application.DTOs.SeatType;
using OscarCinema.Application.DTOs.Ticket;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TicketController(ITicketService ticketService, ILogger<TicketController> logger, IUnitOfWork unitOfWork)
        {
            _ticketService = ticketService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TicketResponse>> Create([FromBody] CreateTicket dto)
        {
            var ticket = await _ticketService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<UserResponse>>> GetAll([FromQuery] PaginationQuery query)
        {
            _logger.LogDebug("Listing all tickets with pagination.");

            var pageResult = await _ticketService.GetAllAsync(query);

            return Ok(pageResult);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllByUser(int userId)
        {
            var tickets = await _ticketService.GetAllByUserIdAsync(userId);
            return Ok(tickets);
        }

        [Authorize(Policy = "AdminOrEmployee")]
        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllBySession(int sessionId)
        {
            var tickets = await _ticketService.GetAllBySessionIdAsync(sessionId);
            return Ok(tickets);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<TicketResponse>> Update(int id, [FromBody] UpdateTicket dto)
        {
            var updatedTicket = await _ticketService.UpdateAsync(id, dto);
            return Ok(updatedTicket);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _ticketService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
