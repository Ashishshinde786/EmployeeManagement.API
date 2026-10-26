
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    // This interface defines the contract for employee database operations.
    // Any class implementing this interface must provide these methods.
    public interface IEmployeeRepository
    {
        // Retrieves a specific page of employees.
        // Supports filtering by department and sorting by a selected column.
        //
        // Returns:
        // Employees    = employee records for the requested page.
        // TotalRecords = total matching records before pagination.
        Task<(List<Employee> Employees, int TotalRecords)> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder);

        // Retrieves one employee using the employee's unique ID.
        // Returns null if no matching employee is found.
        Task<Employee?> GetByIdAsync(int id);

        // Adds a new employee to the repository's data context.
        Task AddAsync(Employee employee);

        // Marks an existing employee for updating.
        Task UpdateAsync(Employee employee);

        // Marks an employee for deletion.
        Task DeleteAsync(Employee employee);

        // Saves pending database changes.
        Task SaveChangesAsync();
    }
}
