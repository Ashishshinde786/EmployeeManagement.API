using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;

namespace EmployeeManagement.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository repository,
            ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET ALL - Pagination + Filtering + Sorting
        public async Task<PagedEmployeeResponse> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder)
        {
            _logger.LogInformation(
                "Fetching employees. Page: {Page}, PageSize: {PageSize}, Department: {Department}, SortBy: {SortBy}, SortOrder: {SortOrder}",
                page,
                pageSize,
                department,
                sortBy,
                sortOrder);

            var result = await _repository.GetPagedAsync(
                page,
                pageSize,
                department,
                sortBy,
                sortOrder);

            var employees = result.Employees
                .Select(employee => new EmployeeResponse
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Department = employee.Department,
                    Salary = employee.Salary
                })
                .ToList();

            var totalPages =
                (int)Math.Ceiling(
                    (double)result.TotalRecords / pageSize);

            return new PagedEmployeeResponse
            {
                Employees = employees,
                Page = page,
                PageSize = pageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = totalPages
            };
        }

        // GET BY ID
        public async Task<EmployeeResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation(
                "Fetching employee with ID {EmployeeId}.",
                id);

            var employee =
                await _repository.GetByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found.",
                    id);

                return null;
            }

            return new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Salary = employee.Salary
            };
        }

        // CREATE
        public async Task<EmployeeResponse> CreateAsync(
            CreateEmployeeRequest request)
        {
            var employee = new Employee
            {
                Name = request.Name,
                Email = request.Email,
                Department = request.Department,
                Salary = request.Salary
            };

            await _repository.AddAsync(employee);

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Employee created successfully with ID {EmployeeId}.",
                employee.Id);

            return new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Salary = employee.Salary
            };
        }

        // UPDATE
        public async Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request)
        {
            _logger.LogInformation(
                "Updating employee with ID {EmployeeId}.",
                id);

            var existingEmployee =
                await _repository.GetByIdAsync(id);

            if (existingEmployee == null)
            {
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found for update.",
                    id);

                return false;
            }

            existingEmployee.Name = request.Name;
            existingEmployee.Email = request.Email;
            existingEmployee.Department = request.Department;
            existingEmployee.Salary = request.Salary;

            await _repository.UpdateAsync(existingEmployee);

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Employee with ID {EmployeeId} updated successfully.",
                id);

            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation(
                "Deleting employee with ID {EmployeeId}.",
                id);

            var employee =
                await _repository.GetByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found for deletion.",
                    id);

                return false;
            }

            await _repository.DeleteAsync(employee);

            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Employee with ID {EmployeeId} deleted successfully.",
                id);

            return true;
        }
    }
}