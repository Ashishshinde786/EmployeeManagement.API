using EmployeeManagement.API.Data;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<string> RegisterAsync(
            RegisterRequest request)
        {
            // Check whether the username already exists.
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == request.Username);

            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }

            // Create a new user object.
            var user = new User
            {
                Username = request.Username,

                // Every user registering through the public API
                // gets the User role by default.
                Role = "User"
            };

            // Hash the password before storing it in the database.
            user.Password = _passwordHasher.HashPassword(
                user,
                request.Password);

            // Add the user to the database.
            _context.Users.Add(user);

            // Save the user.
            await _context.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<string?> LoginAsync(
            LoginRequest request)
        {
            // Find the user using the username provided during login.
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == request.Username);

            // If the username does not exist, login fails.
            if (user == null)
            {
                return null;
            }

            // Verify the entered password against the hashed
            // password stored in the database.
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password);

            // If the password is incorrect, login fails.
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // Password verification was successful.
            // Generate a JWT token for the authenticated user.
            var token = _jwtService.GenerateToken(user);

            // Return the JWT token.
            return token;
        }
    }
}