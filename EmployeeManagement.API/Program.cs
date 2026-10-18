// ============================================================
// REQUIRED NAMESPACES / IMPORTS
// ============================================================

// Contains our Entity Framework Core DbContext.
// AppDbContext is responsible for communicating with SQL Server.
using EmployeeManagement.API.Data;


// Contains our custom global exception handling middleware.
//
// This middleware catches unexpected exceptions from the API
// and returns a standard HTTP error response instead of exposing
// technical exception details to the client.
using EmployeeManagement.API.Middleware;


// Contains the User entity/model.
//
// We use User with ASP.NET Core's PasswordHasher<User>
// to securely hash and verify passwords.
using EmployeeManagement.API.Models;


// Contains Repository interfaces and implementations.
//
// Repository layer is responsible for database-related operations.
//
// Example:
// Controller
//     ↓
// Service
//     ↓
// Repository
//     ↓
// Entity Framework Core
//     ↓
// SQL Server
using EmployeeManagement.API.Repositories;


// Contains Service interfaces and implementations.
//
// Service layer contains business logic.
//
// Example:
// EmployeeController
//      ↓
// IEmployeeService
//      ↓
// EmployeeService
//      ↓
// IEmployeeRepository
//      ↓
// EmployeeRepository
using EmployeeManagement.API.Services;


// ASP.NET Core password hashing functionality.
//
// IPasswordHasher<User> provides methods such as:
//
// HashPassword()
// VerifyHashedPassword()
//
// We use it instead of storing plain-text passwords.
using Microsoft.AspNetCore.Identity;


// Provides JWT Bearer authentication.
//
// JWT authentication allows clients to send a JWT token
// with every protected API request.
using Microsoft.AspNetCore.Authentication.JwtBearer;


// Contains classes required to validate JWT tokens.
//
// TokenValidationParameters defines rules such as:
//
// - Should signature be validated?
// - Should issuer be validated?
// - Should audience be validated?
// - Should expiration be validated?
using Microsoft.IdentityModel.Tokens;


// Used to convert our JWT secret key string into bytes.
//
// Example:
//
// "my-secret-key"
//       ↓
// UTF8 bytes
//       ↓
// SymmetricSecurityKey
using System.Text;


// Provides OpenAPI / Swagger security configuration.
//
// We use this to configure the Swagger "Authorize" button
// for JWT authentication.
using Microsoft.OpenApi;


// Provides Entity Framework Core functionality.
//
// Required for:
// - UseSqlServer()
// - DbContext configuration
using Microsoft.EntityFrameworkCore;


// ============================================================
// CREATE APPLICATION BUILDER
// ============================================================

// WebApplication.CreateBuilder(args) creates the ASP.NET Core
// application builder.
//
// The builder is responsible for preparing:
//
// - Dependency Injection container
// - Configuration
// - Logging
// - Environment information
// - Web server configuration
// - Application services
//
// At this stage, the application is being CONFIGURED.
// It has not started accepting HTTP requests yet.
var builder = WebApplication.CreateBuilder(args);


// ============================================================
// MVC / WEB API CONFIGURATION
// ============================================================

// Register ASP.NET Core MVC Controller support.
//
// This tells ASP.NET Core that our application uses Controllers
// to handle HTTP requests.
//
// Example:
//
// POST /api/employees
//        ↓
// EmployeeController
//
// GET /api/employees/1
//        ↓
// EmployeeController
//
// Without AddControllers(), controller-based APIs
// will not be registered correctly.
builder.Services.AddControllers();


// ============================================================
// DATABASE CONFIGURATION
// ============================================================

// Register Entity Framework Core DbContext.
//
// AppDbContext acts as a bridge between our application
// and SQL Server.
//
// Application
//     ↓
// AppDbContext
//     ↓
// Entity Framework Core
//     ↓
// SQL Server
//
// AddDbContext() also registers AppDbContext in the
// Dependency Injection container.
//
// By default, DbContext is registered with Scoped lifetime.
//
// That means one DbContext instance is normally created
// for each HTTP request.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// ------------------------------------------------------------
// HOW CONNECTION STRING WORKS
// ------------------------------------------------------------
//
// builder.Configuration reads configuration from sources
// such as:
//
// appsettings.json
// appsettings.Development.json
// environment variables
// command-line arguments
//
// Example appsettings.json:
//
// "ConnectionStrings": {
//     "DefaultConnection":
//       "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeDb;Trusted_Connection=True;"
// }
//
// GetConnectionString("DefaultConnection")
// reads the value of:
//
// ConnectionStrings:DefaultConnection
//
// UseSqlServer() tells Entity Framework Core:
//
// "Use SQL Server as the database provider."



// ============================================================
// EMPLOYEE SERVICE AND REPOSITORY
// ============================================================

// Register Employee Service with Dependency Injection.
//
// IEmployeeService = abstraction/interface
// EmployeeService  = actual implementation
//
// Whenever a class asks for:
//
// IEmployeeService
//
// ASP.NET Core DI container automatically provides:
//
// EmployeeService
//
// Example:
//
// public EmployeeController(IEmployeeService service)
// {
//     _service = service;
// }
builder.Services.AddScoped<IEmployeeService, EmployeeService>();


// Register Employee Repository.
//
// Whenever a class asks for:
//
// IEmployeeRepository
//
// ASP.NET Core creates/provides:
//
// EmployeeRepository
//
// Repository is responsible mainly for database access,
// while Service contains business logic.
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();


// ------------------------------------------------------------
// WHY USE INTERFACE + IMPLEMENTATION?
// ------------------------------------------------------------
//
// Instead of:
//
// EmployeeService service = new EmployeeService();
//
// we use:
//
// IEmployeeService
//
// This provides:
//
// - Loose coupling
// - Easier unit testing
// - Dependency Injection
// - Easier replacement of implementation
// - Better maintainability
//
// Architecture:
//
// Controller
//     ↓
// IEmployeeService
//     ↓
// EmployeeService
//     ↓
// IEmployeeRepository
//     ↓
// EmployeeRepository
//     ↓
// AppDbContext
//     ↓
// SQL Server



// ============================================================
// AUTHENTICATION SERVICE
// ============================================================

// Register our custom authentication service.
//
// AuthService contains authentication-related business logic.
//
// Typically it handles:
//
// 1. User registration
// 2. Find user by email/username
// 3. Password verification
// 4. JWT generation
//
// Example login flow:
//
// Client
//   ↓
// POST /api/auth/login
//   ↓
// AuthController
//   ↓
// IAuthService
//   ↓
// AuthService
//   ↓
// Check user from database
//   ↓
// Verify password
//   ↓
// Generate JWT
//   ↓
// Return JWT to client
builder.Services.AddScoped<IAuthService, AuthService>();


// ============================================================
// JWT SERVICE
// ============================================================

// Register JWT service.
//
// JwtService is responsible for creating JWT tokens.
//
// It normally receives:
//
// - User ID
// - Username/email
// - Roles/claims
//
// and creates a digitally signed JWT.
//
// Example:
//
// Login successful
//       ↓
// JwtService
//       ↓
// Create Claims
//       ↓
// Create Signing Credentials
//       ↓
// Create JWT
//       ↓
// Return token
//
// The client then sends this token with future requests.
builder.Services.AddScoped<IJwtService, JwtService>();


// ============================================================
// PASSWORD HASHING
// ============================================================

// Register ASP.NET Core's PasswordHasher.
//
// IPasswordHasher<User>
//        ↓
// PasswordHasher<User>
//
// This service provides secure password hashing.
//
// During registration:
//
// Plain Password
//      ↓
// PasswordHasher
//      ↓
// Password Hash
//      ↓
// Store Hash in Database
//
// Example:
//
// User enters:
// "MyPassword123"
//
// Database stores something similar to:
//
// AQAAAAIAAYagAAAAE...
//
// NOT:
//
// MyPassword123
//
// IMPORTANT:
// We should NEVER store plain-text passwords in the database.
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();


// ------------------------------------------------------------
// PASSWORD LOGIN FLOW
// ------------------------------------------------------------
//
// During login:
//
// User enters:
//
// Email: ashish@gmail.com
// Password: MyPassword123
//
//        ↓
//
// AuthService gets user from DB
//
//        ↓
//
// PasswordHasher.VerifyHashedPassword()
//
//        ↓
//
// Password matches?
//
// YES → Generate JWT
//
// NO → Return Unauthorized
//
// PasswordHasher handles the hashing/verification process
// without us manually implementing password hashing algorithms.



// ============================================================
// JWT AUTHENTICATION
// ============================================================

// Register JWT Bearer authentication.
//
// This tells ASP.NET Core:
//
// "Use JWT Bearer tokens to authenticate API requests."
//
// Client sends:
//
// Authorization: Bearer eyJhbGciOiJIUzI1Ni...
//
// ASP.NET Core extracts the token and validates it.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    // Add JWT Bearer authentication handler.
    //
    // JwtBearerHandler is responsible for processing
    // the Authorization header and validating the JWT.
    .AddJwtBearer(options =>
    {
        // ----------------------------------------------------
        // READ JWT CONFIGURATION
        // ----------------------------------------------------

        // Read the secret key from configuration.
        //
        // Example appsettings.json:
        //
        // "Jwt": {
        //     "Key": "very-long-secret-key",
        //     "Issuer": "EmployeeManagementAPI",
        //     "Audience": "EmployeeManagementClient"
        // }
        var key = builder.Configuration["Jwt:Key"];


        // Read expected token issuer.
        var issuer = builder.Configuration["Jwt:Issuer"];


        // Read expected token audience.
        var audience = builder.Configuration["Jwt:Audience"];


        // ----------------------------------------------------
        // JWT TOKEN VALIDATION RULES
        // ----------------------------------------------------

        // TokenValidationParameters tells ASP.NET Core
        // exactly how the JWT should be validated.
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // ------------------------------------------------
                // 1. VALIDATE SIGNATURE
                // ------------------------------------------------

                // Ensures that the JWT was signed using
                // the expected secret/signing key.
                //
                // This protects against someone modifying
                // the token manually.
                ValidateIssuerSigningKey = true,


                // ------------------------------------------------
                // SIGNING KEY
                // ------------------------------------------------

                // Convert our secret key string into bytes
                // and create a symmetric security key.
                //
                // The same signing key must be used when
                // generating and validating the JWT.
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key!)
                    ),


                // ------------------------------------------------
                // 2. VALIDATE ISSUER
                // ------------------------------------------------

                // Issuer means:
                //
                // "Who created this token?"
                //
                // Example:
                //
                // iss = EmployeeManagementAPI
                //
                // ASP.NET Core checks whether the token's issuer
                // matches the configured issuer.
                ValidateIssuer = true,

                ValidIssuer = issuer,


                // ------------------------------------------------
                // 3. VALIDATE AUDIENCE
                // ------------------------------------------------

                // Audience means:
                //
                // "Who is this token intended for?"
                //
                // Example:
                //
                // aud = EmployeeManagementClient
                //
                // The API checks whether the token was intended
                // for this application/API.
                ValidateAudience = true,

                ValidAudience = audience,


                // ------------------------------------------------
                // 4. VALIDATE TOKEN EXPIRATION
                // ------------------------------------------------

                // Checks whether the JWT has expired.
                //
                // If the token is expired:
                //
                // Authentication fails.
                ValidateLifetime = true,


                // ------------------------------------------------
                // CLOCK SKEW
                // ------------------------------------------------

                // ClockSkew provides tolerance between server
                // clocks when validating token expiration.
                //
                // Normally ASP.NET Core has a default tolerance.
                //
                // Setting it to Zero means:
                //
                // "Do not allow additional expiration time."
                //
                // So if the token is expired, it is immediately
                // considered invalid.
                ClockSkew = TimeSpan.Zero
            };
    });


// ============================================================
// AUTHORIZATION
// ============================================================

// Register ASP.NET Core Authorization services.
//
// Authentication answers:
//
// "WHO ARE YOU?"
//
// Authorization answers:
//
// "ARE YOU ALLOWED TO DO THIS?"
//
// Example:
//
// [Authorize]
// public IActionResult GetEmployees()
//
// The user must be authenticated before accessing
// the endpoint.
//
// Later we can also implement:
//
// [Authorize(Roles = "Admin")]
//
// which means only users with the Admin role can access it.
builder.Services.AddAuthorization();


// ============================================================
// SWAGGER / OPENAPI
// ============================================================

// Register API Explorer.
//
// ASP.NET Core uses this to discover controller actions
// and generate API metadata.
//
// Swagger uses this metadata to display endpoints.
builder.Services.AddEndpointsApiExplorer();


// Configure Swagger/OpenAPI.
builder.Services.AddSwaggerGen(options =>
{
    // ========================================================
    // JWT BEARER SECURITY DEFINITION
    // ========================================================

    // Define JWT Bearer authentication for Swagger.
    //
    // This allows Swagger UI to understand that our API
    // uses JWT authentication.
    //
    // It creates the "Authorize" button in Swagger UI.
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            // Name of the HTTP header.
            //
            // Client sends:
            //
            // Authorization: Bearer <token>
            Name = "Authorization",


            // HTTP authentication scheme.
            Type = SecuritySchemeType.Http,


            // Authentication scheme.
            //
            // IMPORTANT:
            // "bearer" tells Swagger this is Bearer authentication.
            Scheme = "bearer",


            // Tells Swagger that the Bearer token is a JWT.
            BearerFormat = "JWT",


            // Token will be sent through HTTP header.
            In = ParameterLocation.Header,


            // Description displayed in Swagger UI.
            Description =
                "Enter your JWT token. Swagger will send it as: " +
                "Authorization: Bearer <token>"
        });


    // ========================================================
    // JWT SECURITY REQUIREMENT
    // ========================================================

    // Define the JWT security requirement for API operations.
    //
    // This connects our "Bearer" security definition
    // with Swagger API documentation.
    //
    // In simple words:
    //
    // "Swagger knows that this API supports JWT Bearer
    // authentication."
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});


// ============================================================
// BUILD APPLICATION
// ============================================================

// Build the application.
//
// At this point ASP.NET Core creates the WebApplication
// using all the services and configuration registered above.
//
// Before this:
//
// builder.Services...
//
// We were CONFIGURING services.
//
// After this:
//
// var app = builder.Build();
//
// We have the actual application object that will
// process HTTP requests.
var app = builder.Build();


// ============================================================
// HTTP REQUEST PIPELINE
// ============================================================
//
// From this point onward we configure the middleware pipeline.
//
// Every incoming HTTP request travels through this pipeline.
//
// Example:
//
// HTTP Request
//      ↓
// Global Exception Middleware
//      ↓
// Swagger
//      ↓
// HTTPS
//      ↓
// Authentication
//      ↓
// Authorization
//      ↓
// Controller
//      ↓
// Service
//      ↓
// Repository
//      ↓
// Database
//
// The order of middleware is important.



// ============================================================
// GLOBAL EXCEPTION HANDLING
// ============================================================

// Add our custom global exception middleware.
//
// This middleware catches unhandled exceptions
// that occur during request processing.
//
// Example:
//
// Controller
//     ↓
// Service
//     ↓
// Database error
//     ↓
// Exception
//     ↓
// GlobalExceptionMiddleware
//     ↓
// Standard error response
//
// Without global exception handling, clients may receive
// inconsistent error responses.
//
// A production API should avoid exposing internal
// exception details such as:
//
// - Database connection information
// - Stack traces
// - Internal class names
// - SQL details
//
// Instead, we can return something like:
//
// {
//     "statusCode": 500,
//     "message": "An unexpected error occurred."
// }
app.UseMiddleware<GlobalExceptionMiddleware>();


// ============================================================
// SWAGGER UI
// ============================================================

// Enable Swagger only when the application is running
// in Development environment.
//
// Environment can be:
//
// Development
// Staging
// Production
//
// In Development:
//
// Swagger JSON endpoint is enabled.
// Swagger UI is enabled.
if (app.Environment.IsDevelopment())
{
    // Generates OpenAPI JSON document.
    app.UseSwagger();


    // Provides interactive Swagger UI.
    //
    // We can test APIs directly from the browser.
    app.UseSwaggerUI();
}


// ============================================================
// HTTPS REDIRECTION
// ============================================================

// Redirect HTTP requests to HTTPS.
//
// Example:
//
// http://localhost:5079/api/employees
//
// gets redirected to:
//
// https://localhost:7226/api/employees
//
// HTTPS encrypts communication between client and server.
//
// This is especially important when sending:
//
// - Passwords
// - JWT tokens
// - Personal information
// - Other sensitive data
app.UseHttpsRedirection();


// ============================================================
// AUTHENTICATION
// ============================================================

// Authentication determines WHO the user is.
//
// This middleware checks whether the request contains
// a valid authentication token.
//
// For JWT authentication it looks for:
//
// Authorization: Bearer <JWT>
//
// Example request:
//
// GET /api/employees
//
// Authorization: Bearer eyJhbGciOiJIUzI1Ni...
//
// The JWT middleware then:
//
// 1. Extracts token
// 2. Validates signature
// 3. Validates issuer
// 4. Validates audience
// 5. Validates expiration
// 6. Creates an authenticated user/ClaimsPrincipal
//
// If valid:
//
// HttpContext.User
//
// contains the authenticated user's claims.
app.UseAuthentication();


// ============================================================
// AUTHORIZATION
// ============================================================

// Authorization determines whether the authenticated
// user is allowed to access the requested resource.
//
// Example:
//
// [Authorize]
// [HttpGet]
// public IActionResult GetEmployees()
//
// If the user is not authenticated:
//
// → 401 Unauthorized
//
// If the user is authenticated but does not have
// required permissions/role:
//
// → 403 Forbidden
//
// Example:
//
// [Authorize(Roles = "Admin")]
//
// Only Admin users are allowed.
app.UseAuthorization();


// ============================================================
// MAP CONTROLLERS
// ============================================================

// Map controller routes to the ASP.NET Core request pipeline.
//
// Example:
//
// [ApiController]
// [Route("api/[controller]")]
// public class EmployeesController : ControllerBase
//
// [HttpGet]
//
// becomes:
//
// GET /api/employees
//
// MapControllers() tells ASP.NET Core:
//
// "Use attribute-routed controllers to handle requests."
app.MapControllers();


// ============================================================
// START APPLICATION
// ============================================================

// Start the ASP.NET Core application.
//
// Kestrel web server starts listening for HTTP/HTTPS requests.
//
// Example:
//
// https://localhost:7226
//
// Once app.Run() executes, the application starts
// accepting incoming requests.
//
// app.Run() normally blocks until the application
// is stopped.
app.Run();