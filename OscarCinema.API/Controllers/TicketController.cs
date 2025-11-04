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

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;

        }

        [HttpPost]
        public async Task<ActionResult<TicketResponseDTO>> Create([FromBody] CreateTicketDTO dto)
        {
            try
            {
                var createdTicket = await _ticketService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdTicket.Id },
                    createdTicket);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponseDTO>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAll()
        {
            var ticketList = await _ticketService.GetAllAsync();

            return Ok(ticketList);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAllByUserId(int userId)
        {
            var ticketList = await _ticketService.GetAllByUserIdAsync(userId);

            return Ok(ticketList);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetAllBySessionId(int sessionId)
        {
            var ticketList = await _ticketService.GetAllBySessionIdAsync(sessionId);

            return Ok(ticketList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TicketResponseDTO>> Update(int id, [FromBody] UpdateTicketDTO dto)
        {
            try
            {
                var updatedTicket = await _ticketService.UpdateAsync(id, dto);
                return Ok(updatedTicket);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _ticketService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Ticket with ID {id} not found");

            return NoContent();
        }
    }
}

