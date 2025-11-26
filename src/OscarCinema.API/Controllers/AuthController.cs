using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Application.Services;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Identity;
using System.Security.Claims;

namespace OscarCinema.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, IUserService userService, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cria ApplicationUser (Identity)
            var appUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                Role = request.Role
            };

            var identityResult = await _userManager.CreateAsync(appUser, request.Password);
            if (!identityResult.Succeeded) return BadRequest(identityResult.Errors);

            var roleResult = await _userManager.AddToRoleAsync(appUser, request.Role.ToString().ToUpper());
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser);
                return BadRequest(roleResult.Errors);
            }

            // Cria usuário no domínio usando UserService
            try
            {
                var createUserDto = new CreateUser
                {
                    ApplicationUserId = appUser.Id,
                    Name = request.Name,
                    DocumentNumber = request.DocumentNumber,
                    Email = request.Email,
                    Role = request.Role
                };

                await _userService.CreateAsync(createUserDto);
            }
            catch (InvalidOperationException ex)
            {
                await _userManager.DeleteAsync(appUser);
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                await _userManager.DeleteAsync(appUser);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create domain user.");
            }

            //Gera o token
            var token = await _tokenService.CreateToken(appUser.Id, appUser.Email, appUser.UserName);
            return Ok(new { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUser request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var appUser = await _userManager.FindByEmailAsync(request.Email);
            if (appUser == null || !await _userManager.CheckPasswordAsync(appUser, request.Password))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                return Unauthorized();
            }

            var token = await _tokenService.CreateToken(
                appUser.Id,
                appUser.Email,
                appUser.UserName
            );

            _logger.LogInformation("User logged in successfully: {Email} (ID: {UserId})", request.Email, appUser.Id);

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok("Password changed successfully.");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var appUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (appUserIdClaim == null)
                return Unauthorized();

            if (!int.TryParse(appUserIdClaim, out int appUserId))
                return Unauthorized();

            var identityUser = await _userManager.FindByIdAsync(appUserId.ToString());
            if (identityUser == null)
                return NotFound();

            var domainUser = await _userService.GetByApplicationUserIdAsync(appUserId);
            if (domainUser == null)
                return NotFound();

            return Ok(new
            {
                Id = domainUser.Id,
                Name = domainUser.Name,
                Email = domainUser.Email,
                DocumentNumber = domainUser.DocumentNumber.Number,
                Role = domainUser.Role
            });
        }

    }
}
