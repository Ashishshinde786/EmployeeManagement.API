using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}