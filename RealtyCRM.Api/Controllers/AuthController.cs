using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealtyCRM.Api.Models;
using RealtyCRM.Api.Services;

namespace RealtyCRM.Api.Controllers
{
    /// <summary>
    /// Контроллер для регистрации и авторизации пользователей.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = new User
                {
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Email = request.Email,
                    Role = request.Role,
                    Preferences = request.Preferences ?? string.Empty,
                    CommissionRate = request.CommissionRate ?? 0
                };

                var createdUser = await _authService.RegisterAsync(user, request.Password);
                return Ok(new UserDto(createdUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла внутренняя ошибка при регистрации.");
            }
        }

        /// <summary>
        /// Вход пользователя в систему.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authService.AuthenticateAsync(request.Email, request.Password);
                if (user == null)
                {
                    return Unauthorized("Неверная почта или пароль.");
                }

                return Ok(new UserDto(user));
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла внутренняя ошибка при входе.");
            }
        }
    }

    public record RegisterRequest(
        string Email, 
        string Password, 
        string FullName, 
        string Phone, 
        string Role, 
        string? Preferences, 
        decimal? CommissionRate);

    public record LoginRequest(string Email, string Password);

    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Preferences { get; set; }
        public decimal CommissionRate { get; set; }

        public UserDto(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Phone = user.Phone;
            Email = user.Email;
            Role = user.Role;
            Preferences = user.Preferences;
            CommissionRate = user.CommissionRate;
        }
    }
}
