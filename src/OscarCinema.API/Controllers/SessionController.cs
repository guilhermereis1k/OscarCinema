using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Session;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Validation;

[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateSession dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var session = await _sessionService.CreateAsync(
                dto.MovieId, dto.RoomId, dto.ExhibitionTypeId, dto.StartTime, dto.DurationMinutes
            );
            return Ok(session);
        }
        catch (DomainExceptionValidation ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateSession dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var session = await _sessionService.UpdateAsync(
                id, dto.MovieId, dto.RoomId, dto.ExhibitionTypeId, dto.StartTime, dto.DurationMinutes
            );
            return Ok(session);
        }
        catch (DomainExceptionValidation ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var session = await _sessionService.GetByIdAsync(id);
        if (session == null) return NotFound();
        return Ok(session);
    }

    [HttpPost("{id}/finish")]
    public async Task<ActionResult> Finish(int id)
    {
        try
        {
            await _sessionService.FinishSessionAsync(id);
            return NoContent();
        }
        catch (DomainExceptionValidation ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
