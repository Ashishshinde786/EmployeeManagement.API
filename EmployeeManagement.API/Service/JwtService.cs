using EmployeeManagement.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            // Read JWT settings from appsettings.json.
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiryMinutes =
                int.Parse(_configuration["Jwt:ExpiryMinutes"]!);

            // Convert the secret key into bytes.
            var keyBytes = Encoding.UTF8.GetBytes(key!);

            // Create the security key used to sign the JWT.
            var securityKey = new SymmetricSecurityKey(keyBytes);

            // Create signing credentials using HMAC SHA-256.
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            // Claims represent information about the authenticated user.
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),

                // Store the user's role inside the JWT.
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create the JWT token.
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            // Convert the JWT object into a string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}