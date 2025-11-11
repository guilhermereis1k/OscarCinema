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
        private readonly ILogger<SessionController> _logger;

        public SessionController(ISessionService sessionService, ILogger<SessionController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<SessionResponseDTO>> Create([FromBody] CreateSessionDTO dto)
        {
            _logger.LogInformation("Creating new session for movie {MovieId} in room {RoomId} at {SessionDate}",
                dto.MovieId, dto.RoomId, dto.StartTime);

            var createdSession = await _sessionService.CreateAsync(dto);

            _logger.LogInformation("Session created with ID: {Id}", createdSession.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdSession.Id },
                createdSession);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SessionResponseDTO>> GetById(int id)
        {
            _logger.LogDebug("Searching session by ID: {Id}", id);

            var session = await _sessionService.GetByIdAsync(id);

            if (session == null)
            {
                _logger.LogWarning("Session not found: {Id}", id);
                return NotFound();
            }

            return Ok(session);
        }

        [HttpGet("movie/{id}")]
        public async Task<ActionResult<IEnumerable<SessionResponseDTO>>> GetAllByMovieId(int id)
        {
            _logger.LogDebug("Getting all sessions for movie ID: {MovieId}", id);

            var sessionList = await _sessionService.GetAllByMovieIdAsync(id);

            if (sessionList == null)
            {
                _logger.LogWarning("No sessions found for movie ID: {MovieId}", id);
                return NotFound();
            }

            _logger.LogDebug("Returning {Count} sessions for movie ID: {MovieId}", sessionList.Count(), id);
            return Ok(sessionList);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionResponseDTO>>> GetAll()
        {
            _logger.LogDebug("Listing all sessions");

            var sessionList = await _sessionService.GetAllAsync();

            _logger.LogDebug("Returning {Count} sessions", sessionList.Count());

            return Ok(sessionList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SessionResponseDTO>> Update(int id, [FromBody] UpdateSessionDTO dto)
        {
            _logger.LogInformation("Updating session ID: {Id} with data: {@Dto}", id, dto);

            var updatedSession = await _sessionService.UpdateAsync(id, dto);

            _logger.LogInformation("Session updated successfully: {Id}", id);

            return Ok(updatedSession);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting session: {Id}", id);

            var deleted = await _sessionService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete session not found: {Id}", id);
                return NotFound($"Session with ID {id} not found");
            }

            _logger.LogInformation("Session deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

