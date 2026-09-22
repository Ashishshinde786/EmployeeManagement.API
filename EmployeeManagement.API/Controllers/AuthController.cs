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

        // ============================================================
        // REGISTER
        // ============================================================

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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


        // ============================================================
        // LOGIN
        // ============================================================

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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