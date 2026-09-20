using EmployeeManagement.API.DTOs;

namespace EmployeeManagement.API.Services
{
    public interface IEmployeeService
    {
        Task<PagedEmployeeResponse> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder);

        Task<EmployeeResponse?> GetByIdAsync(int id);

        Task<EmployeeResponse> CreateAsync(
            CreateEmployeeRequest request);

        Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request);

        Task<bool> DeleteAsync(int id);
    }
}