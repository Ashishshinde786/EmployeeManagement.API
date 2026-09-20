// Import classes required for Entity Framework Core database configuration.
using EmployeeManagement.API.Data;

// Import our custom global exception handling middleware.
using EmployeeManagement.API.Middleware;

// Import the User model used for password hashing.
using EmployeeManagement.API.Models;

// Import repository interfaces and implementations.
using EmployeeManagement.API.Repositories;

// Import service interfaces and implementations.
using EmployeeManagement.API.Services;

// Provides ASP.NET Core password hashing functionality.
using Microsoft.AspNetCore.Identity;

// Provides JWT Bearer authentication.
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Provides JWT token validation classes.
using Microsoft.IdentityModel.Tokens;

// Provides OpenAPI / Swagger security classes.
using Microsoft.OpenApi;

// Provides UTF8 encoding for converting the JWT secret into bytes.
using System.Text;

// Provides Entity Framework Core functionality.
using Microsoft.EntityFrameworkCore;


// ============================================================
// CREATE APPLICATION BUILDER
// ============================================================

var builder = WebApplication.CreateBuilder(args);


// ============================================================
// MVC / WEB API CONFIGURATION
// ============================================================

// Register Controller support.
builder.Services.AddControllers();


// ============================================================
// DATABASE CONFIGURATION
// ============================================================

// Register Entity Framework Core DbContext.
//
// AppDbContext manages communication between
// the application and SQL Server.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// ============================================================
// EMPLOYEE SERVICE AND REPOSITORY
// ============================================================

// Register Employee Service.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();


// Register Employee Repository.
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();


// ============================================================
// AUTHENTICATION SERVICE
// ============================================================

// Register our custom authentication service.
//
// AuthService handles:
// - User registration
// - Password verification
// - JWT generation
builder.Services.AddScoped<IAuthService, AuthService>();


// ============================================================
// JWT SERVICE
// ============================================================

// Register JWT service.
//
// JwtService generates JWT tokens after successful login.
builder.Services.AddScoped<IJwtService, JwtService>();


// ============================================================
// PASSWORD HASHING
// ============================================================

// Register ASP.NET Core password hashing service.
//
// Passwords are hashed before being stored in SQL Server.
//
// Never store plain-text passwords in the database.
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();


// ============================================================
// JWT AUTHENTICATION
// ============================================================

// Register JWT Bearer authentication.
//
// ASP.NET Core will look for:
//
// Authorization: Bearer <JWT_TOKEN>
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Read JWT configuration from appsettings.json.
        var key = builder.Configuration["Jwt:Key"];
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];

        // Configure JWT validation rules.
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // Validate the JWT signature.
                ValidateIssuerSigningKey = true,

                // Use the same secret key that was
                // used when generating the JWT.
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key!)
                    ),

                // Validate the token issuer.
                ValidateIssuer = true,
                ValidIssuer = issuer,

                // Validate the token audience.
                ValidateAudience = true,
                ValidAudience = audience,

                // Validate token expiration.
                ValidateLifetime = true,

                // Don't allow additional clock-skew time.
                ClockSkew = TimeSpan.Zero
            };
    });


// ============================================================
// AUTHORIZATION
// ============================================================

// Register authorization services.
//
// [Authorize] uses this configuration to determine
// whether a user can access a protected endpoint.
builder.Services.AddAuthorization();


// ============================================================
// SWAGGER / OPENAPI
// ============================================================

// Enable API Explorer.
builder.Services.AddEndpointsApiExplorer();


// Configure Swagger/OpenAPI.
builder.Services.AddSwaggerGen(options =>
{
    // --------------------------------------------------------
    // JWT BEARER SECURITY DEFINITION
    // --------------------------------------------------------

    // Define JWT Bearer authentication for Swagger.
    //
    // This creates the Authorize button in Swagger UI.
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            // JWT uses HTTP Bearer authentication.
            Type = SecuritySchemeType.Http,

            // Bearer must be lowercase here.
            Scheme = "bearer",

            // Tell Swagger that the Bearer token is a JWT.
            BearerFormat = "JWT",

            // Token is sent through the HTTP header.
            In = ParameterLocation.Header,

            Description =
                "Enter your JWT token. Swagger will send it as: " +
                "Authorization: Bearer <token>"
        });


    // --------------------------------------------------------
    // JWT SECURITY REQUIREMENT
    // --------------------------------------------------------

    // Tell Swagger that the Bearer scheme should be
    // applied to the API operations.
    //
    // Swashbuckle 10 requires the document parameter
    // and OpenApiSecuritySchemeReference.
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

var app = builder.Build();


// ============================================================
// GLOBAL EXCEPTION HANDLING
// ============================================================

// Add our global exception middleware.
//
// Unexpected exceptions are caught here and converted
// into a standard API error response.
app.UseMiddleware<GlobalExceptionMiddleware>();


// ============================================================
// SWAGGER UI
// ============================================================

// Enable Swagger only in Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// ============================================================
// HTTPS
// ============================================================

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();


// ============================================================
// AUTHENTICATION
// ============================================================

// Authenticate the incoming request.
//
// ASP.NET Core checks for:
//
// Authorization: Bearer <JWT>
//
// and validates the token using AddJwtBearer().
app.UseAuthentication();


// ============================================================
// AUTHORIZATION
// ============================================================

// Check whether the authenticated user
// is authorized to access the endpoint.
app.UseAuthorization();


// ============================================================
// MAP CONTROLLERS
// ============================================================

// Map all controller endpoints.
app.MapControllers();


// ============================================================
// START APPLICATION
// ============================================================

app.Run();