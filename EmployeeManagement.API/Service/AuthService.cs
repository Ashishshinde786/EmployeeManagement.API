using EmployeeManagement.API.Data;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Exceptions;
using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Services
{
    // Implements the authentication operations defined in IAuthService.
    public class AuthService : IAuthService
    {
        // Used to access users stored in the database.
        private readonly AppDbContext _context;

        // Used to hash passwords and verify entered passwords.
        private readonly IPasswordHasher<User> _passwordHasher;

        // Used to generate JWT tokens after successful login.
        private readonly IJwtService _jwtService;

        // Dependency injection provides the required objects.
        public AuthService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        // Registers a new user.
        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            // Check whether the username is already present.
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == request.Username);

            // Do not allow duplicate usernames.
            if (usernameExists)
            {
                throw new ConflictException("Username already exists.");
            }

            // Create a new user object.
            var user = new User
            {
                Username = request.Username,

                // Public registration always creates a normal user,
                // not an administrator.
                Role = "User"
            };

            // Hash the password before storing it in the database.
            // Never store the original plain-text password.
            user.Password = _passwordHasher.HashPassword(
                user,
                request.Password);

            // Add the new user to EF Core's change tracker.
            _context.Users.Add(user);

            try
            {
                // Save the new user to the database.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Two requests could check the username at the same time.
                // A database unique constraint should prevent duplicates.
                var nowExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Username == request.Username);

                if (nowExists)
                {
                    throw new ConflictException("Username already exists.");
                }

                // If the error was unrelated to a duplicate username,
                // preserve and rethrow the original exception.
                throw;
            }

            return "User registered successfully.";
        }

        // Authenticates a user and returns a JWT if successful.
        public async Task<string?> LoginAsync(LoginRequest request)
        {
            // Find the user by username.
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            // If no user exists, login fails.
            if (user == null)
            {
                return null;
            }

            // Compare the entered password with the stored password hash.
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password);

            // Return null if the password is incorrect.
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // If the password hash needs upgrading, generate a new hash.
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(
                    user,
                    request.Password);

                await _context.SaveChangesAsync();
            }

            // Login succeeded. Generate and return the JWT.
            return _jwtService.GenerateToken(user);
        }

        // Changes the password of an existing user.
        public async Task<bool> ChangePasswordAsync(
            int userId,
            ChangePasswordRequest request)
        {
            // Find the user by their database ID.
            var user = await _context.Users.FindAsync(userId);

            // Return false if the user does not exist.
            if (user == null)
            {
                return false;
            }

            // Verify the current password before allowing a change.
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.CurrentPassword);

            // Reject the request if the current password is incorrect.
            if (result == PasswordVerificationResult.Failed)
            {
                return false;
            }

            // Hash the new password before storing it.
            user.Password = _passwordHasher.HashPassword(
                user,
                request.NewPassword);

            // Persist the updated password hash.
            await _context.SaveChangesAsync();

            return true;
        }
    }
}