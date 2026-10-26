using EmployeeManagement.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.API.Services
{
    // Implements the IJwtService interface.
    // Responsible for creating JWT tokens for authenticated users.
    public class JwtService : IJwtService
    {
        // Provides access to settings from appsettings.json,
        // environment variables, user-secrets, and other configuration sources.
        private readonly IConfiguration _configuration;

        // IConfiguration is injected through dependency injection.
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Creates a JWT token using the logged-in user's information.
        public string GenerateToken(User user)
        {
            // Read JWT settings from configuration.
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // A signing key is required to create a signed token.
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "Jwt:Key is missing. Add it to appsettings.json or user-secrets.");
            }

            // Read token lifetime from configuration.
            // Use 60 minutes if the setting is missing or invalid.
            var expiryMinutes =
                int.TryParse(
                    _configuration["Jwt:ExpiryMinutes"],
                    out var minutes)
                && minutes > 0
                    ? minutes
                    : 60;

            // Convert the secret key into bytes and create a security key.
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            // Define how the token will be digitally signed.
            // HMAC SHA-256 uses the shared secret key.
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            // Add information about the user to the token.
            var claims = new[]
            {
                // Unique identifier of the user.
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                // Username of the authenticated user.
                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                // Role used for role-based authorization.
                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            // Create the JWT with its issuer, audience, claims,
            // expiration time, and signing credentials.
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            // Convert the JWT object into the string sent to the client.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}