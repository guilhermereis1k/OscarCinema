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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> Create([FromBody] CreateUserDTO dto)
        {
            _logger.LogInformation("Creating new user: {Email}", dto.Email);

            var createdUser = await _userService.CreateAsync(dto);

            _logger.LogInformation("User created with ID: {Id}", createdUser.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdUser.Id },
                createdUser);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetById(int id)
        {
            _logger.LogDebug("Searching user by ID: {Id}", id);

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Id}", id);
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll()
        {
            _logger.LogDebug("Listing all users");

            var userList = await _userService.GetAllAsync();

            _logger.LogDebug("Returning {Count} users", userList.Count());

            return Ok(userList);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponseDTO>> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            _logger.LogInformation("Updating user ID: {Id}", id);

            var updatedUser = await _userService.UpdateAsync(id, dto);

            _logger.LogInformation("User updated successfully: {Id}", id);

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting user: {Id}", id);

            var deleted = await _userService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempt to delete user not found: {Id}", id);
                return NotFound($"User with ID {id} not found");
            }

            _logger.LogInformation("User deleted successfully: {Id}", id);
            return NoContent();
        }
    }
}

