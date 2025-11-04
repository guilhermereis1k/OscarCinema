using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Repositories;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;

        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> Create([FromBody] CreateUserDTO dto)
        {
            try
            {
                var createdUser = await _userService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdUser.Id },
                    createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll()
        {
            var userList = await _userService.GetAllAsync();

            return Ok(userList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponseDTO>> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                var updatedUser = await _userService.UpdateAsync(id, dto);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"User with ID {id} not found");

            return NoContent();
        }
    }
}

