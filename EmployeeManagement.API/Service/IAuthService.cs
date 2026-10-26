using EmployeeManagement.API.DTOs;

namespace EmployeeManagement.API.Services
{
    // Defines the contract for authentication-related operations.
    public interface IAuthService
    {
        // Registers a user.
        // Returns a success message.
        // Throws ConflictException if the username already exists.
        Task<string> RegisterAsync(RegisterRequest request);

        // Authenticates a user.
        // Returns a JWT token if login succeeds,
        // or null if the username or password is invalid.
        Task<string?> LoginAsync(LoginRequest request);

        // Changes a user's password.
        // userId identifies the user.
        // request contains the current and new passwords.
        // Returns true if the password was changed;
        // otherwise, returns false.
        Task<bool> ChangePasswordAsync(
            int userId,
            ChangePasswordRequest request);
    }
}