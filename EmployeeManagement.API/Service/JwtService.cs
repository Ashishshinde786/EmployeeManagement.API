
// ============================================================
// REQUIRED NAMESPACES
// ============================================================

// Provides access to the User entity/model.
//
// JwtService receives a User object and uses information
// from that user to create JWT claims.
using EmployeeManagement.API.Models;


// Provides JWT security functionality.
//
// This namespace contains classes such as:
//
// - SymmetricSecurityKey
// - SigningCredentials
// - SecurityAlgorithms
//
// These are used to securely sign the JWT token.
using Microsoft.IdentityModel.Tokens;


// Provides JWT token-related classes.
//
// We use this namespace for:
//
// - JwtSecurityToken
// - JwtSecurityTokenHandler
//
// These classes are used to create and convert JWT tokens.
using System.IdentityModel.Tokens.Jwt;


// Provides Claim and ClaimTypes.
//
// Claims represent information about the authenticated user,
// such as username and role.
using System.Security.Claims;


// Provides Encoding functionality.
//
// We use UTF8 encoding to convert the secret JWT key from
// a string into a byte array.
using System.Text;


// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains service classes.
//
// JwtService is responsible for creating JWT tokens.
//
// Authentication flow:
//
// Login Request
//      ↓
// AuthController
//      ↓
// AuthService
//      ↓
// Verify Username + Password
//      ↓
// JwtService
//      ↓
// Generate JWT
//      ↓
// Return JWT to Client
namespace EmployeeManagement.API.Services
{
    // ========================================================
    // JWT SERVICE
    // ========================================================

    // JwtService implements IJwtService.
    //
    // IJwtService defines the contract:
    //
    //     string GenerateToken(User user);
    //
    // This class contains the actual implementation for
    // generating the JWT token.
    public class JwtService : IJwtService
    {
        // ====================================================
        // CONFIGURATION
        // ====================================================

        // IConfiguration provides access to application
        // configuration values.
        //
        // In our application, JWT settings are stored in:
        //
        // appsettings.json
        //
        // Example:
        //
        // "Jwt": {
        //     "Key": "...",
        //     "Issuer": "EmployeeManagement.API",
        //     "Audience": "EmployeeManagement.Client",
        //     "ExpiryMinutes": 60
        // }
        //
        // We use _configuration to read these values.
        private readonly IConfiguration _configuration;


        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // IConfiguration is injected through Dependency
        // Injection.
        //
        // ASP.NET Core automatically provides the
        // IConfiguration object.
        public JwtService(IConfiguration configuration)
        {
            // Store the injected configuration object so that
            // GenerateToken() can read JWT settings.
            _configuration = configuration;
        }


        // ============================================================
        // GENERATE JWT TOKEN
        // ============================================================

        // Generates a JWT token for the authenticated user.
        //
        // The User object contains information such as:
        //
        // - Id
        // - Username
        // - Role
        //
        // This information can be placed inside JWT claims.
        //
        // The generated token is returned as a string.
        public string GenerateToken(User user)
        {
            // =================================================
            // READ JWT CONFIGURATION
            // =================================================

            // Read the secret signing key from appsettings.json.
            //
            // This key is used to digitally sign the JWT.
            //
            // The same key must be available to the API when
            // validating incoming JWT tokens.
            var key = _configuration["Jwt:Key"];


            // Read the token issuer.
            //
            // Issuer identifies the system/application that
            // issued the JWT.
            //
            // Example:
            //
            // EmployeeManagement.API
            var issuer = _configuration["Jwt:Issuer"];


            // Read the token audience.
            //
            // Audience identifies the application or clients
            // for which the token is intended.
            var audience = _configuration["Jwt:Audience"];


            // Read how long the JWT should remain valid.
            //
            // Example:
            //
            // ExpiryMinutes = 60
            //
            // The generated token will expire after 60 minutes.
            //
            // int.Parse() converts the configuration value,
            // which is read as a string, into an integer.
            //
            // The ! tells the compiler that we expect this
            // configuration value to be present.
            var expiryMinutes =
                int.Parse(_configuration["Jwt:ExpiryMinutes"]!);


            // =================================================
            // CONVERT SECRET KEY TO BYTES
            // =================================================

            // The JWT signing mechanism requires the secret
            // key in byte-array form.
            //
            // Encoding.UTF8.GetBytes() converts:
            //
            // "MySecretKey..."
            //
            // into:
            //
            // byte[]
            //
            // This byte array is then used to create the
            // security key.
            var keyBytes = Encoding.UTF8.GetBytes(key!);


            // =================================================
            // CREATE SECURITY KEY
            // =================================================

            // Create a symmetric security key using the secret
            // key bytes.
            //
            // "Symmetric" means the same secret key is used
            // for signing and validating the token.
            //
            // During token creation:
            //
            // Secret Key
            //      ↓
            // Sign JWT
            //
            // During token validation:
            //
            // Same Secret Key
            //      ↓
            // Verify JWT signature
            var securityKey =
                new SymmetricSecurityKey(keyBytes);


            // =================================================
            // CREATE SIGNING CREDENTIALS
            // =================================================

            // SigningCredentials tells the JWT system:
            //
            // 1. Which security key should be used?
            // 2. Which cryptographic algorithm should be used?
            //
            // HmacSha256 is the signing algorithm used here.
            //
            // The signature helps the API verify that the token
            // was generated using the expected secret key and
            // has not been modified.
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);


            // =================================================
            // CREATE JWT CLAIMS
            // =================================================

            // Claims are pieces of information stored inside
            // the JWT payload.
            //
            // Claims can represent information about the
            // authenticated user.
            //
            // In this application we are storing:
            //
            // - Username
            // - Role
            var claims = new[]
            {
                // Store the authenticated user's username.
                //
                // ClaimTypes.Name is commonly used by ASP.NET
                // Core to represent the user's name.
                new Claim(
                    ClaimTypes.Name,
                    user.Username),


                // Store the user's role inside the JWT.
                //
                // This is important because your application
                // uses role-based authorization.
                //
                // Example:
                //
                // [Authorize(Roles = "Admin")]
                //
                // ASP.NET Core can read this role claim and
                // determine whether the authenticated user
                // has the required role.
                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };


            // =================================================
            // CREATE JWT TOKEN
            // =================================================

            // Create the actual JWT object.
            //
            // The token contains:
            //
            // issuer
            //     → Who created the token.
            //
            // audience
            //     → Who the token is intended for.
            //
            // claims
            //     → User information.
            //
            // expires
            //     → Token expiration time.
            //
            // signingCredentials
            //     → Information required to digitally sign
            //       the token.
            var token = new JwtSecurityToken(
                // Identify the application that issued the token.
                issuer: issuer,

                // Identify the intended audience of the token.
                audience: audience,

                // Add username and role claims.
                claims: claims,

                // Set the token expiration time.
                //
                // DateTime.UtcNow gets the current UTC time.
                //
                // AddMinutes() adds the configured number of
                // minutes to calculate the expiration time.
                expires: DateTime.UtcNow.AddMinutes(
                    expiryMinutes),

                // Use the HMAC SHA-256 signing credentials.
                signingCredentials: credentials);


            // =================================================
            // CONVERT JWT OBJECT TO STRING
            // =================================================

            // JwtSecurityToken is a .NET object.
            //
            // The client needs the actual JWT string, usually
            // in this format:
            //
            // header.payload.signature
            //
            // JwtSecurityTokenHandler converts the token object
            // into that standard JWT string format.
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}

