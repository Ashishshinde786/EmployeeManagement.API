
using EmployeeManagement.API.DTOs;

namespace EmployeeManagement.API.Services
{
    // Defines the business operations that an employee service must provide.
    // EmployeeService implements this interface.
    public interface IEmployeeService
    {
        // Retrieves a page of employees with optional filtering and sorting.
        // Returns employee data along with pagination metadata.
        Task<PagedEmployeeResponse> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder);

        // Retrieves one employee by ID.
        // Returns null if the employee does not exist.
        Task<EmployeeResponse?> GetByIdAsync(int id);

        // Creates an employee using the data received in the request DTO.
        // Returns the newly created employee as a response DTO.
        Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);

        // Updates an existing employee using the supplied ID and request data.
        // Returns true if the update succeeds, or false if the employee is missing.
        Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request);

        // Deletes an employee by ID.
        // Returns true if deletion succeeds, or false if the employee is missing.
        Task<bool> DeleteAsync(int id);
    }
}
