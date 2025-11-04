using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;

        }

        [HttpPost]
        public async Task<ActionResult<SessionResponseDTO>> Create([FromBody] CreateSessionDTO dto)
        {
            try
            {
                var createdSession = await _sessionService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdSession.Id },
                    createdSession);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SessionResponseDTO>> GetById(int id)
        {
            var session = await _sessionService.GetByIdAsync(id);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        [HttpGet("movie/{id}")]
        public async Task<ActionResult<IEnumerable<SessionResponseDTO>>> GetAllByMovieId(int id)
        {
            var sessionList = await _sessionService.GetAllByMovieIdAsync(id);

            if (sessionList == null)
                return NotFound();

            return Ok(sessionList);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionResponseDTO>>> GetAll()
        {
            var sessionList = await _sessionService.GetAllAsync();

            return Ok(sessionList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SessionResponseDTO>> Update(int id, [FromBody] UpdateSessionDTO dto)
        {
            try
            {
                var updatedSession = await _sessionService.UpdateAsync(id, dto);
                return Ok(updatedSession);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _sessionService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Session with ID {id} not found");

            return NoContent();
        }
    }
}

