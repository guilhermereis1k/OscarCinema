using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.Pagination;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
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

        [Authorize(Policy = "JustAdmin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(int id)
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

        [Authorize(Policy = "JustAdmin")]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<UserResponse>>> GetAll([FromQuery] PaginationQuery query)
        {
            _logger.LogDebug("Listing all users with pagination.");

            var pageResult = await _userService.GetAllAsync(query);

            _logger.LogDebug(
                "Returning {Count} items for the current page.", pageResult.Data.Count());

            return Ok(pageResult);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UpdateUser dto)
        {
            _logger.LogInformation("Updating user ID: {Id}", id);

            var updatedUser = await _userService.UpdateAsync(id, dto);

            _logger.LogInformation("User updated successfully: {Id}", id);

            return Ok(updatedUser);
        }

        [Authorize]
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

