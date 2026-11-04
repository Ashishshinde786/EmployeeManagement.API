
using EmployeeManagement.API.DTOs;

namespace EmployeeManagement.API.Services
{
    public interface IDepartmentService
    {
        // Get all active departments.
        Task<List<DepartmentResponse>> GetAllAsync();

        // Get department by ID.
        Task<DepartmentResponse?> GetByIdAsync(int id);

        // Create department.
        Task<DepartmentResponse> CreateAsync(
            CreateDepartmentRequest request);

        // Update department.
        Task<DepartmentResponse?> UpdateAsync(
            int id,
            UpdateDepartmentRequest request);

        // Soft delete department.
        Task<bool> DeleteAsync(int id);
    }
}
