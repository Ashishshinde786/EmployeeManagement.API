using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IEmployeeRepository
    {
        Task<(List<Employee> Employees, int TotalRecords)> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder);

        Task<Employee?> GetByIdAsync(int id);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);

        Task SaveChangesAsync();
    }
}