using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            // Register a new user.
            var result = await _authService.RegisterAsync(request);

            return Ok(new
            {
                message = result
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            // Validate the username and password.
            var result = await _authService.LoginAsync(request);

            // If authentication fails, return HTTP 401 Unauthorized.
            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            // Login was successful.
            // The result contains the generated JWT token.
            return Ok(new
            {
                token = result
            });
        }
    }
}