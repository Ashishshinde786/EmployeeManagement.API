
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Exceptions;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;

namespace EmployeeManagement.API.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        // Get all active departments.
        public async Task<List<DepartmentResponse>> GetAllAsync()
        {
            var departments = await _repository.GetAllAsync();

            return departments.Select(d => MapToResponse(d)).ToList();
        }

        // Get department by ID.
        public async Task<DepartmentResponse?> GetByIdAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null || !department.IsActive)
                return null;

            return MapToResponse(department);
        }

        // Create a new department.
        public async Task<DepartmentResponse> CreateAsync(
            CreateDepartmentRequest request)
        {
            var departmentName = request.Name.Trim();

            // Check duplicate department name.
            var exists = await _repository.ExistsByNameAsync(departmentName);

            if (exists)
            {
                throw new ConflictException(
                    "Department name already exists.");
            }

            var department = new Department
            {
                Name = departmentName,
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return MapToResponse(department);
        }

        // Update an existing department.
        public async Task<DepartmentResponse?> UpdateAsync(
            int id,
            UpdateDepartmentRequest request)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null || !department.IsActive)
                return null;

            var departmentName = request.Name.Trim();

            // Check duplicate name excluding the current department.
            var exists = await _repository.ExistsByNameAsync(
                departmentName,
                id);

            if (exists)
            {
                throw new ConflictException(
                    "Another department already uses this name.");
            }

            department.Name = departmentName;
            department.Description = request.Description?.Trim();
            department.IsActive = request.IsActive;
            department.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return MapToResponse(department);
        }

        // Soft delete department.
        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null || !department.IsActive)
                return false;

            department.IsActive = false;
            department.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return true;
        }

        // Convert Department entity into response DTO.
        private static DepartmentResponse MapToResponse(
            Department department)
        {
            return new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };
        }
    }
}
