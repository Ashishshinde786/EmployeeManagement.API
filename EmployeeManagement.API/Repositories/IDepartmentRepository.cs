
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IDepartmentRepository
    {
        // Get all active departments.
        Task<List<Department>> GetAllAsync();

        // Get department by ID.
        Task<Department?> GetByIdAsync(int id);

        // Check whether department name already exists.
        // During update, excludeDepartmentId prevents matching the same record.
        Task<bool> ExistsByNameAsync(
            string name,
            int? excludeDepartmentId = null);

        // Add a new department.
        Task AddAsync(Department department);

        // Save database changes.
        Task SaveChangesAsync();
    }
}
