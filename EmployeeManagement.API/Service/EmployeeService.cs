
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;

namespace EmployeeManagement.API.Services
{
    // Implements the employee business logic defined by IEmployeeService.
    public class EmployeeService : IEmployeeService
    {
        // Provides access to employee database operations through the interface.
        private readonly IEmployeeRepository _repository;

        // Provides logging for important operations and problems.
        private readonly ILogger<EmployeeService> _logger;

        // Constructor injection:
        // ASP.NET Core supplies the registered repository and logger.
        public EmployeeService(
            IEmployeeRepository repository,
            ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Converts an Employee database entity into an EmployeeResponse DTO.
        // Keeping this mapping in one place avoids repeating the same code.
        private static EmployeeResponse MapToResponse(Employee e) => new()
        {
            Id = e.Id,
            Name = e.Name,
            Email = e.Email,
            Department = e.Department,
            Salary = e.Salary,
            PhoneNumber = e.PhoneNumber,
            JoiningDate = e.JoiningDate,
            JobTitle = e.JobTitle
        };

        // Retrieves employees with pagination, filtering and sorting.
        public async Task<PagedEmployeeResponse> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder)
        {
            // Log the search parameters to help diagnose application behaviour.
            _logger.LogInformation(
                "Fetching employees. Page: {Page}, PageSize: {PageSize}, Department: {Department}, SortBy: {SortBy}, SortOrder: {SortOrder}",
                page, pageSize, department, sortBy, sortOrder);

            // Ask the repository to retrieve the requested records.
            // The repository handles database filtering, sorting and pagination.
            var result = await _repository.GetPagedAsync(
                page, pageSize, department, sortBy, sortOrder);

            // Convert each Employee entity into an EmployeeResponse DTO.
            var employees = result.Employees
                .Select(MapToResponse)
                .ToList();

            // Calculate the number of pages.
            // Ceiling rounds up if the division produces a fraction.
            var totalPages =
                (int)Math.Ceiling(
                    (double)result.TotalRecords / pageSize);

            // Return the employee list together with pagination information.
            return new PagedEmployeeResponse
            {
                Employees = employees,
                Page = page,
                PageSize = pageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = totalPages
            };
        }

        // Retrieves a single employee and returns its response DTO.
        // Returns null when the employee does not exist.
        public async Task<EmployeeResponse?> GetByIdAsync(int id)
        {
            // Record which employee is being requested.
            _logger.LogInformation(
                "Fetching employee with ID {EmployeeId}.", id);

            // Retrieve the employee through the repository.
            var employee = await _repository.GetByIdAsync(id);

            // Handle the case where the requested ID does not exist.
            if (employee == null)
            {
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found.", id);

                return null;
            }

            // Return a DTO instead of exposing the database entity directly.
            return MapToResponse(employee);
        }

        // Creates a new employee from the incoming request DTO.
        public async Task<EmployeeResponse> CreateAsync(
            CreateEmployeeRequest request)
        {
            // Copy the request data into a new database entity.
            // The database-generated Id is assigned when the entity is saved.
            var employee = new Employee
            {
                Name = request.Name,
                Email = request.Email,
                Department = request.Department,
                Salary = request.Salary,
                PhoneNumber = request.PhoneNumber,

                // Both properties are nullable DateTime values.
                JoiningDate = request.JoiningDate,
                JobTitle = request.JobTitle
            };

            // Mark the new employee for insertion.
            await _repository.AddAsync(employee);

            // Persist the pending insertion to the database.
            await _repository.SaveChangesAsync();

            // Log success after the save operation completes.
            _logger.LogInformation(
                "Employee created successfully with ID {EmployeeId}.",
                employee.Id);

            // Return the created employee as a response DTO.
            return MapToResponse(employee);
        }

        // Updates an existing employee.
        // Returns false if the employee does not exist.
        public async Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request)
        {
            _logger.LogInformation(
                "Updating employee with ID {EmployeeId}.", id);

            // First retrieve the existing employee.
            var existing = await _repository.GetByIdAsync(id);

            // Do not attempt to update a nonexistent employee.
            if (existing == null)
            {
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found for update.",
                    id);

                return false;
            }

            // Copy the incoming request values onto the existing entity.
            existing.Name = request.Name;
            existing.Email = request.Email;
            existing.Department = request.Department;
            existing.Salary = request.Salary;
            existing.PhoneNumber = request.PhoneNumber;
            existing.JoiningDate = request.JoiningDate;
            existing.JobTitle = request.JobTitle;

            // Mark the entity for updating.
            await _repository.UpdateAsync(existing);

            // Persist the update to the database.
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Employee with ID {EmployeeId} updated successfully.", id);

            return true;
        }

        // Deletes an employee by ID.
        // Returns false if the employee does not exist.
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation(
                "Deleting employee with ID {EmployeeId}.", id);

            // Retrieve the employee before attempting deletion.
            var employee = await _repository.GetByIdAsync(id);

            // Return false if there is no employee with this ID.
            if (employee == null)
            {
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found for deletion.",
                    id);

                return false;
            }

            // Mark the employee for deletion.
            await _repository.DeleteAsync(employee);

            // Persist the deletion to the database.
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Employee with ID {EmployeeId} deleted successfully.", id);

            return true;
        }
    }
}
