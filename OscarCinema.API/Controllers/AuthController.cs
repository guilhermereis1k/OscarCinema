using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OscarCinema.Application.DTOs.User;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Identity;

namespace OscarCinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO request)
        {
            _logger.LogInformation("Register attempt for email: {Email}", request.Email);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                Role = request.Role
            };

            var result = await _userManager.CreateAsync(appUser, request.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to register user: {Email}", request.Email);
                return BadRequest(result.Errors);
            }

            var roleName = request.Role.ToString().ToUpper();
            await _userManager.AddToRoleAsync(appUser, roleName);

            var freshAppUser = await _userManager.FindByEmailAsync(request.Email);

            if (freshAppUser == null)
            {
                _logger.LogError("User created but not found: {Email}", request.Email);
                return StatusCode(500, "User creation failed");
            }

            var token = await _tokenService.CreateToken(
                (int)freshAppUser.Id,
                freshAppUser.Email,
                freshAppUser.UserName
            );

            _logger.LogInformation("User registered successfully: {Email} (ID: {UserId}) with role: {Role}",
                request.Email, freshAppUser.Id, roleName);

            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO request)
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

    }
}
