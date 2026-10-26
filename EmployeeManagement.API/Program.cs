using EmployeeManagement.API.Data;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

// Create the application builder.
// It provides configuration, logging and dependency injection.
var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. REGISTER CONTROLLERS
// ============================================================

// Enables MVC controllers to handle incoming API requests.
builder.Services.AddControllers();

// ============================================================
// 2. DATABASE CONFIGURATION
// ============================================================

// Register AppDbContext and configure it to use SQL Server.
// The connection string is read from configuration.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// 3. DEPENDENCY INJECTION
// ============================================================

// When IEmployeeService is requested, provide EmployeeService.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// When IEmployeeRepository is requested, provide EmployeeRepository.
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// Register authentication business logic.
builder.Services.AddScoped<IAuthService, AuthService>();

// Register JWT token generation.
builder.Services.AddScoped<IJwtService, JwtService>();

// Register ASP.NET Core's password hashing implementation.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ============================================================
// 4. JWT SECRET KEY CONFIGURATION
// ============================================================

// Read the signing key from configuration.
// In production, provide it using a secure secret store or environment variable.
var jwtKey = builder.Configuration["Jwt:Key"];
var usingGeneratedDevKey = false;

// A missing or short key is handled differently in development
// and production.
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    if (builder.Environment.IsDevelopment())
    {
        // Generate a temporary random key for development.
        jwtKey = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(48));

        // Make the generated key available to JwtService
        // and to the JWT validation configuration below.
        builder.Configuration["Jwt:Key"] = jwtKey;

        usingGeneratedDevKey = true;
    }
    else
    {
        // Do not start production with a missing or short key.
        throw new InvalidOperationException(
            "Jwt:Key is missing or shorter than 32 characters. " +
            "Provide it through the environment variable Jwt__Key " +
            "or your host's secret store.");
    }
}

// Read the expected issuer and audience.
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// ============================================================
// 5. JWT AUTHENTICATION
// ============================================================

// Configure JWT Bearer authentication.
// ASP.NET Core will use this scheme to authenticate requests
// containing a Bearer token.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = true;

        // Define how incoming JWT tokens must be validated.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Verify the token signature using the configured key.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)),

            // Verify who issued the token.
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            // Verify that the token is intended for this API/client.
            ValidateAudience = true,
            ValidAudience = jwtAudience,

            // Reject expired tokens.
            ValidateLifetime = true,

            // Do not allow an additional clock-skew grace period.
            ClockSkew = TimeSpan.Zero,

            // Map claims to the identity's name and role.
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

// Register authorization services.
// Authorization determines what an authenticated user can access.
builder.Services.AddAuthorization();

// ============================================================
// 6. RATE LIMITING
// ============================================================

// Read the maximum permitted authentication requests per minute.
// Use 5 if the setting is missing.
var authPermitLimit =
    builder.Configuration.GetValue<int?>(
        "RateLimit:AuthPermitPerMinute") ?? 5;

builder.Services.AddRateLimiter(options =>
{
    // Return HTTP 429 when a request exceeds the limit.
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    // Define a fixed-window rate-limiting policy called "auth".
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            // Maintain a separate limit for each remote IP address.
            httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                // Maximum requests allowed in the window.
                PermitLimit = authPermitLimit,

                // Reset the window every minute.
                Window = TimeSpan.FromMinutes(1),

                // Do not queue excess requests.
                QueueLimit = 0
            }));
});

// ============================================================
// 7. SWAGGER CONFIGURATION
// ============================================================

// Makes API endpoint information available to API Explorer.
builder.Services.AddEndpointsApiExplorer();

// Register Swagger/OpenAPI generation.
builder.Services.AddSwaggerGen(options =>
{
    // Tell Swagger how to send JWT Bearer tokens.
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter your JWT token. Swagger will send it as: " +
                "Authorization: Bearer <token>"
        });

    // Add the Bearer security requirement to the OpenAPI document.
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// ============================================================
// 8. BUILD THE APPLICATION
// ============================================================

var app = builder.Build();

// Warn during development if a temporary signing key was generated.
// Tokens signed with that temporary key will not remain valid
// after the application restarts.
if (usingGeneratedDevKey)
{
    app.Logger.LogWarning(
        "No Jwt:Key configured. Using a temporary random key for this run. " +
        "Tokens will stop working when the app restarts.");
}

// ============================================================
// 9. CONFIGURE THE HTTP REQUEST PIPELINE
// ============================================================

// Handle exceptions that pass through the remaining pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable Swagger only in development.
// In other environments, enable HTTP Strict Transport Security.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Apply rate-limiting middleware.
app.UseRateLimiter();

// Identify the caller from the supplied authentication credentials.
app.UseAuthentication();

// Check whether the authenticated caller is allowed to access
// the requested endpoint.
app.UseAuthorization();

// Map controller actions to HTTP endpoints.
app.MapControllers();

// Start the application and begin listening for requests.
app.Run();