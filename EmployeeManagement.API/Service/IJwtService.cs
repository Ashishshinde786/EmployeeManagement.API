using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Services
{
    // Defines the contract for JWT token generation.
    public interface IJwtService
    {
        // Accepts a user object and returns the generated JWT token.
        string GenerateToken(User user);
    }
}