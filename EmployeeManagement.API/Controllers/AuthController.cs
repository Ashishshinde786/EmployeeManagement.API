
// ============================================================
// NAMESPACES / IMPORTS
// ============================================================

// Contains our DTO classes.
//
// DTO = Data Transfer Object.
//
// RegisterRequest and LoginRequest are used to receive
// only the data required by the API.
//
// We should avoid directly exposing database entities
// through API requests.
using EmployeeManagement.API.DTOs;


// Contains authentication service interfaces
// and implementations.
//
// The controller depends on IAuthService rather than
// directly implementing authentication/business logic.
using EmployeeManagement.API.Services;


// Provides ASP.NET Core MVC functionality.
//
// ControllerBase, IActionResult, ApiController,
// HttpPost, Route, etc. come from ASP.NET Core MVC.
using Microsoft.AspNetCore.Mvc;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.Controllers
{
    // ========================================================
    // AUTH CONTROLLER
    // ========================================================

    // [ApiController] tells ASP.NET Core that this class
    // is an API controller.
    //
    // It provides several useful API features automatically:
    //
    // 1. Automatic model validation
    // 2. Automatic HTTP 400 response for invalid models
    // 3. Better parameter binding
    // 4. API-specific behavior
    //
    // Example:
    //
    // If RegisterRequest has:
    //
    // [Required]
    // public string Email { get; set; }
    //
    // and the client doesn't provide Email,
    // ASP.NET Core can automatically return 400 Bad Request.
    //
    [ApiController]


    // [Route("api/[controller]")] defines the base route
    // for this controller.
    //
    // [controller] is replaced by the controller class name
    // without the "Controller" suffix.
    //
    // AuthController
    //      ↓
    // Auth
    //
    // Therefore:
    //
    // Base route:
    //
    // /api/auth
    //
    // Then the action route is added after that.
    //
    // Example:
    //
    // [HttpPost("register")]
    //
    // becomes:
    //
    // POST /api/auth/register
    //
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // ====================================================
        // AUTH SERVICE DEPENDENCY
        // ====================================================

        // The controller does not directly perform:
        //
        // - Database operations
        // - Password hashing
        // - Password verification
        // - JWT generation
        //
        // Instead, it delegates authentication business logic
        // to IAuthService.
        //
        // This follows Separation of Concerns.
        private readonly IAuthService _authService;


        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // ASP.NET Core Dependency Injection automatically
        // provides an implementation of IAuthService.
        //
        // Program.cs contains:
        //
        // builder.Services.AddScoped<IAuthService, AuthService>();
        //
        // Therefore ASP.NET Core does:
        //
        // IAuthService
        //      ↓
        // AuthService
        //      ↓
        // AuthController
        //
        public AuthController(IAuthService authService)
        {
            // Store the injected service in a private field
            // so the controller actions can use it.
            _authService = authService;
        }


        // ====================================================
        // REGISTER
        // ====================================================

        // [HttpPost("register")] means this action handles
        // HTTP POST requests.
        //
        // Because the controller route is:
        //
        // /api/auth
        //
        // and the action route is:
        //
        // /register
        //
        // the complete endpoint is:
        //
        // POST /api/auth/register
        //
        // POST is generally used when creating a new resource.
        [HttpPost("register")]


        // Swagger/OpenAPI documentation:
        //
        // If registration succeeds, the API can return:
        //
        // 200 OK
        //
        [ProducesResponseType(StatusCodes.Status200OK)]


        // The API may return:
        //
        // 400 Bad Request
        //
        // for invalid request data.
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        // The API may return:
        //
        // 409 Conflict
        //
        // when a business conflict occurs.
        //
        // Example:
        //
        // Email already exists.
        //
        [ProducesResponseType(StatusCodes.Status409Conflict)]


        // ====================================================
        // REGISTER ACTION
        // ====================================================

        // This method handles the registration request.
        //
        // RegisterRequest contains the information required
        // to create a new user.
        //
        // Example request:
        //
        // {
        //     "username": "ashish",
        //     "email": "ashish@gmail.com",
        //     "password": "Password123"
        // }
        //
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            // =================================================
            // CALL AUTHENTICATION SERVICE
            // =================================================

            // The controller delegates the actual registration
            // business logic to the service layer.
            //
            // Controller:
            //     "I received the request."
            //
            // Service:
            //     "I will perform the registration logic."
            //
            // The service may:
            //
            // 1. Check whether the user already exists.
            // 2. Hash the password.
            // 3. Create the User entity.
            // 4. Save the user to SQL Server.
            //
            var result = await _authService.RegisterAsync(request);


            // =================================================
            // RETURN SUCCESS RESPONSE
            // =================================================

            // Return HTTP 200 OK.
            //
            // The response is returned as JSON.
            //
            // Example:
            //
            // {
            //     "message": "User registered successfully."
            // }
            //
            return Ok(new
            {
                message = result
            });
        }


        // ====================================================
        // LOGIN
        // ====================================================

        // This endpoint handles:
        //
        // POST /api/auth/login
        //
        // Login is a POST request because credentials are
        // sent to the server in the request body.
        [HttpPost("login")]


        // Swagger documentation:
        //
        // Successful login:
        //
        // 200 OK
        //
        [ProducesResponseType(StatusCodes.Status200OK)]


        // Invalid request:
        //
        // 400 Bad Request
        //
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        // Invalid credentials:
        //
        // 401 Unauthorized
        //
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        // ====================================================
        // LOGIN ACTION
        // ====================================================

        // LoginRequest contains the credentials supplied
        // by the client.
        //
        // Example:
        //
        // {
        //     "username": "ashish",
        //     "password": "Password123"
        // }
        //
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            // =================================================
            // AUTHENTICATE USER
            // =================================================

            // Send the login request to the authentication
            // service.
            //
            // AuthService is responsible for:
            //
            // 1. Finding the user in the database.
            // 2. Verifying the password.
            // 3. Generating a JWT if credentials are valid.
            //
            // The controller doesn't need to know how
            // password hashing or JWT generation works.
            var result = await _authService.LoginAsync(request);


            // =================================================
            // LOGIN FAILED
            // =================================================

            // If LoginAsync returns null, authentication failed.
            //
            // Possible reason:
            //
            // - User does not exist
            // - Password is incorrect
            //
            // We return:
            //
            // HTTP 401 Unauthorized
            //
            if (result == null)
            {
                // Return a JSON response:
                //
                // {
                //     "message": "Invalid username or password."
                // }
                //
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }


            // =================================================
            // LOGIN SUCCESSFUL
            // =================================================

            // If result is not null, authentication succeeded.
            //
            // The result contains the generated JWT token.
            //
            // The client should store the token and send it
            // with future protected API requests.
            //
            // Example:
            //
            // Authorization: Bearer <JWT>
            //
            return Ok(new
            {
                token = result
            });
        }
    }
}
