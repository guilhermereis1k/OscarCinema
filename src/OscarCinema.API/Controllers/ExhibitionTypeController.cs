using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OscarCinema.Application.DTOs.ExhibitionType;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitionTypeController : ControllerBase
    {
        private readonly IExhibitionTypeService _exhibitionTypeService;
        private readonly ILogger<ExhibitionTypeController> _logger;

        public ExhibitionTypeController(IExhibitionTypeService exhibitionTypeService, ILogger<ExhibitionTypeController> logger)
        {
            _exhibitionTypeService = exhibitionTypeService;
            _logger = logger;
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpPost]
        public async Task<ActionResult<ExhibitionTypeResponse>> Create([FromBody] CreateExhibitionType dto)
        {
            _logger.LogInformation("Creating new exhibition type: {Name}", dto.Name);

            var createdExhibitionType = await _exhibitionTypeService.CreateAsync(dto);

            _logger.LogInformation("Exhibition type created with ID: {Id}", createdExhibitionType.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdExhibitionType.Id },
                createdExhibitionType);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ExhibitionTypeResponse>> GetById(int id)
        {
            _logger.LogDebug("Fetching exhibition type by ID: {Id}", id);

            var exhibitionType = await _exhibitionTypeService.GetByIdAsync(id);

            if (exhibitionType == null)
            {
                _logger.LogWarning("Exhibition type not found: {Id}", id);
                return NotFound();
            }

            return Ok(exhibitionType);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<ExhibitionTypeResponse>>> GetAll([FromQuery] PaginationQuery query)
        {
            _logger.LogDebug("Listing all exhibition types with pagination.");

            var pageResult = await _exhibitionTypeService.GetAllAsync(query);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExhibitionTypeResponse>> Update(int id, [FromBody] UpdateExhibitionType dto)
        {
            _logger.LogInformation("Updating exhibition type ID: {Id} with data: {@Dto}", id, dto);

            var updatedExhibitionType = await _exhibitionTypeService.UpdateAsync(id, dto);

            _logger.LogInformation("Exhibition type updated successfully: {Id}", id);

            return Ok(updatedExhibitionType);
        }

        [Authorize(Policy = "JustAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting exhibition type: {Id}", id);

            var deleted = await _exhibitionTypeService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempted to delete exhibition type not found: {Id}", id);
                return NotFound($"ExhibitionType with ID {id} not found");
            }

            _logger.LogInformation("Exhibition type deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

