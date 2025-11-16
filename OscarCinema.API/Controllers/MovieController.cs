using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.Pagination;
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
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<MovieResponse>> Create([FromBody] CreateMovie dto)
        {
            _logger.LogInformation("Creating new movie: {Title}", dto.Title);

            var createdMovie = await _movieService.CreateAsync(dto);

            _logger.LogInformation("Movie created with ID: {Id}", createdMovie.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdMovie.Id },
                createdMovie);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponse>> GetById(int id)
        {
            _logger.LogDebug("Searching movie by ID: {Id}", id);

            var movie = await _movieService.GetByIdAsync(id);

            if (movie == null)
            {
                _logger.LogWarning("Movie not found: {Id}", id);
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResult<MovieResponse>>> GetAll([FromQuery] PaginationQuery query)
        {
            _logger.LogDebug("Listing all movies with pagination.");

            var pageResult = await _movieService.GetAllAsync(query);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MovieResponse>> Update(int id, [FromBody] UpdateMovie dto)
        {
            _logger.LogInformation("Updating movie ID: {Id} with data: {@Dto}", id, dto);

            var updatedMovie = await _movieService.UpdateAsync(id, dto);

            _logger.LogInformation("Movie updated successfully: {Id}", id);

            return Ok(updatedMovie);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting movie: {Id}", id);

            var deleted = await _movieService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete movie not found: {Id}", id);
                return NotFound($"Movie with ID {id} not found");
            }

            _logger.LogInformation("Movie deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

