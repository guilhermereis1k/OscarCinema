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

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;

        }

        [HttpPost]
        public async Task<ActionResult<MovieResponseDTO>> Create([FromBody] CreateMovieDTO dto)
        {
            try
            {
                var createdMovie = await _movieService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdMovie.Id },
                    createdMovie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponseDTO>> GetById(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);

            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieResponseDTO>>> GetAll()
        {
            var movieList = await _movieService.GetAllAsync();

            return Ok(movieList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MovieResponseDTO>> Update(int id, [FromBody] UpdateMovieDTO dto)
        {
            try
            {
                var updatedMovie = await _movieService.UpdateAsync(id, dto);
                return Ok(updatedMovie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _movieService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Movie with ID {id} not found");

            return NoContent();
        }
    }
}

