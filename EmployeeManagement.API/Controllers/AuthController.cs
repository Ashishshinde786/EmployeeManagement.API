// Imports request and response DTOs such as RegisterRequest,
// LoginRequest, and ChangePasswordRequest.
using EmployeeManagement.API.DTOs;

// Imports service interfaces, including IAuthService.
// The controller uses this service to perform authentication operations.
using EmployeeManagement.API.Services;

// Provides the [Authorize] attribute to protect endpoints
// that require the user to be authenticated.
using Microsoft.AspNetCore.Authorization;

// Provides ControllerBase, routing attributes, and HTTP response helpers
// such as Ok(), BadRequest(), and Unauthorized().
using Microsoft.AspNetCore.Mvc;

// Allows us to apply rate-limiting policies to this controller.
using Microsoft.AspNetCore.RateLimiting;

// Provides methods for reading claims from the authenticated user.
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    // Tells ASP.NET Core that this class handles API requests
    // and enables automatic request validation for DTO validation attributes.
    [ApiController]

    // Builds the URL using the controller name.
    // AuthController becomes "auth", so the base route is /api/auth.
    [Route("api/[controller]")]

    // Indicates that this controller returns JSON responses.
    [Produces("application/json")]

    // Applies the "auth" rate-limiting policy configured in Program.cs.
    // This helps restrict repeated requests to authentication endpoints.
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        // Holds a reference to the authentication service.
        // The service contains the actual registration, login,
        // and password-changing logic.
        private readonly IAuthService _authService;

        // Constructor: ASP.NET Core's dependency injection system
        // supplies an implementation of IAuthService when the controller
        // is created for an incoming request.
        public AuthController(IAuthService authService)
        {
            // Store the injected service for use in the controller methods.
            _authService = authService;
        }

        // Endpoint: POST /api/auth/register
        // Used to register a new user account.
        [HttpPost("register")]

        // Documents the possible HTTP responses for API consumers
        // and Swagger. These attributes do not themselves generate
        // the corresponding responses.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // The request body is bound to RegisterRequest.
            // With [ApiController], ASP.NET Core automatically validates
            // its data annotations, such as [Required] and [StringLength].
            //
            // Pass the validated request to the authentication service.
            // The service checks whether the username exists, hashes the
            // password, and saves the new user in the database.
            //
            // await waits asynchronously for the operation to finish.
            var result = await _authService.RegisterAsync(request);

            // Return HTTP 200 OK with a JSON message.
            // Example: { "message": "User registered successfully." }
            return Ok(new { message = result });
        }

        // Endpoint: POST /api/auth/login
        // Used to authenticate a user and obtain a JWT.
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Ask the authentication service to verify the username
            // and password.
            //
            // If the credentials are valid, the service returns a JWT.
            // If they are invalid, it returns null.
            var token = await _authService.LoginAsync(request);

            // If no token was generated, authentication failed.
            if (token == null)
            {
                // Return HTTP 401 Unauthorized with an error message.
                // The same generic message is used for an invalid
                // username or password.
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            // Return HTTP 200 OK containing the generated JWT.
            // Example: { "token": "eyJhbGciOi..." }
            //
            // The client can send this token in the Authorization header
            // when calling protected endpoints.
            return Ok(new { token });
        }

        // Endpoint: POST /api/auth/change-password
        // Allows an authenticated user to change their own password.
        [HttpPost("change-password")]

        // Requires a valid authenticated identity.
        // An unauthenticated request is normally rejected with HTTP 401
        // before this action method executes.
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordRequest request)
        {
            // Read the user's ID from the authenticated identity's claims.
            //
            // JwtService adds this claim when creating the JWT:
            // ClaimTypes.NameIdentifier = user.Id.ToString()
            //
            // If authentication succeeds, ASP.NET Core builds the User
            // ClaimsPrincipal from the validated token.
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            // Convert the claim value from a string into an integer.
            // If the claim is missing or is not a valid integer,
            // we cannot safely identify the account.
            if (!int.TryParse(userIdValue, out var userId))
            {
                // Return HTTP 401 Unauthorized if the user ID is invalid.
                return Unauthorized();
            }

            // Pass the authenticated user's ID and the requested
            // password changes to the service.
            //
            // The service verifies the current password, hashes the
            // new password, and saves the updated hash to the database.
            var changed = await _authService.ChangePasswordAsync(
                userId, request);

            // The service returns false if the user was not found
            // or the current password was incorrect.
            if (!changed)
            {
                // Return HTTP 400 Bad Request with an error message.
                return BadRequest(new
                {
                    message = "Current password is incorrect."
                });
            }

            // Return HTTP 200 OK when the password change succeeds.
            return Ok(new
            {
                message = "Password changed successfully."
            });
        }
    }
}