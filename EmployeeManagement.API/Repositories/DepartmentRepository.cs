
using EmployeeManagement.API.Data;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all active departments.
        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        // Get department by ID.
        // Tracking is enabled by default because this entity may be updated.
        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // Check duplicate department name.
        public async Task<bool> ExistsByNameAsync(
            string name,
            int? excludeDepartmentId = null)
        {
            var normalizedName = name.Trim().ToLower();

            var query = _context.Departments
                .Where(d => d.Name.ToLower() == normalizedName);

            if (excludeDepartmentId.HasValue)
            {
                query = query.Where(
                    d => d.Id != excludeDepartmentId.Value);
            }

            return await query.AnyAsync();
        }

        // Add a new department.
        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        // Save changes to database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
