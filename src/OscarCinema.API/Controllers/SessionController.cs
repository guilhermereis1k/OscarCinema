using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [Authorize(Policy = "AdminOrEmployee")]
        [HttpPost("create")]
        public async Task<ActionResult<SessionResponse>> Create([FromBody] CreateSession dto)
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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionResponse>> GetById(int id)
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

        [AllowAnonymous]
        [HttpGet("movie/{id}")]
        public async Task<ActionResult<PaginationResult<SessionResponse>>> GetAllByMovieId([FromQuery] PaginationQuery query, int id)
        {
            _logger.LogDebug("Listing all sessions with pagination");

            var pageResult = await _sessionService.GetAllByMovieIdAsync(query, id);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<SessionResponse>>> GetAll([FromQuery] PaginationQuery query, int id)
        {
            _logger.LogDebug("Listing all sessions with pagination.");

            var pageResult = await _sessionService.GetAllAsync(query);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [Authorize(Policy = "AdminOrEmployee")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SessionResponse>> Update(int id, [FromBody] UpdateSession dto)
        {
            _logger.LogInformation("Updating session ID: {Id} with data: {@Dto}", id, dto);

            var updatedSession = await _sessionService.UpdateAsync(id, dto);

            _logger.LogInformation("Session updated successfully: {Id}", id);

            return Ok(updatedSession);
        }

        [Authorize(Policy = "AdminOrEmployee")]
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

