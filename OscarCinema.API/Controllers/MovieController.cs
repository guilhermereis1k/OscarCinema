using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

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
        public async Task<ActionResult<MovieResponseDTO>> Create([FromBody] CreateMovieDTO dto)
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
        public async Task<ActionResult<MovieResponseDTO>> GetById(int id)
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
        public async Task<ActionResult<IEnumerable<MovieResponseDTO>>> GetAll()
        {
            _logger.LogDebug("Listing all movies");

            var movieList = await _movieService.GetAllAsync();

            _logger.LogDebug("Returning {Count} movies", movieList.Count());

            return Ok(movieList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MovieResponseDTO>> Update(int id, [FromBody] UpdateMovieDTO dto)
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

