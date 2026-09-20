using EmployeeManagement.API.Data;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL - Filtering + Sorting + Pagination
        public async Task<(List<Employee> Employees, int TotalRecords)> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder)
        {
            // Start query
            var query = _context.Employees.AsQueryable();

            // -----------------------------------------
            // 1. FILTERING
            // -----------------------------------------

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(e =>
                    e.Department == department);
            }

            // -----------------------------------------
            // 2. COUNT FILTERED RECORDS
            // -----------------------------------------

            var totalRecords =
                await query.CountAsync();

            // -----------------------------------------
            // 3. SORTING
            // -----------------------------------------

            if (string.Equals(
                sortBy,
                "name",
                StringComparison.OrdinalIgnoreCase))
            {
                query = string.Equals(
                    sortOrder,
                    "desc",
                    StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Name)
                    : query.OrderBy(e => e.Name);
            }
            else if (string.Equals(
                sortBy,
                "salary",
                StringComparison.OrdinalIgnoreCase))
            {
                query = string.Equals(
                    sortOrder,
                    "desc",
                    StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Salary)
                    : query.OrderBy(e => e.Salary);
            }
            else if (string.Equals(
                sortBy,
                "department",
                StringComparison.OrdinalIgnoreCase))
            {
                query = string.Equals(
                    sortOrder,
                    "desc",
                    StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Department)
                    : query.OrderBy(e => e.Department);
            }
            else
            {
                // Default sorting
                query = query.OrderBy(e => e.Id);
            }

            // -----------------------------------------
            // 4. PAGINATION
            // -----------------------------------------

            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (
                employees,
                totalRecords
            );
        }

        // GET BY ID
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .FindAsync(id);
        }

        // CREATE
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees
                .AddAsync(employee);
        }

        // UPDATE
        public Task UpdateAsync(Employee employee)
        {
            _context.Employees
                .Update(employee);

            return Task.CompletedTask;
        }

        // DELETE
        public Task DeleteAsync(Employee employee)
        {
            _context.Employees
                .Remove(employee);

            return Task.CompletedTask;
        }

        // SAVE
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}