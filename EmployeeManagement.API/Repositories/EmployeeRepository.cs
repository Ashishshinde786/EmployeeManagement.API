
using EmployeeManagement.API.Data;
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Repositories
{
    // EmployeeRepository contains the database operations for Employee.
    // It implements IEmployeeRepository, which defines the operations
    // that an employee repository must provide.
    public class EmployeeRepository : IEmployeeRepository
    {
        // AppDbContext provides access to the database through EF Core.
        // readonly means this field can be assigned only during declaration
        // or inside the constructor.
        private readonly AppDbContext _context;

        // Constructor injection:
        // ASP.NET Core's Dependency Injection container provides AppDbContext.
        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves employees using filtering, sorting, and pagination.
        //
        // Returns:
        // 1. Employees: the employees for the requested page.
        // 2. TotalRecords: the total matching employees before pagination.
        //
        // The ? after string means the parameter can be null.
        public async Task<(List<Employee> Employees, int TotalRecords)> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder)
        {
            // Access the Employees table through AppDbContext.
            //
            // AsNoTracking() improves read-only query performance because
            // EF Core does not track the returned entities for changes.
            //
            // AsQueryable() allows us to build the query conditionally.
            // The database query is generally executed when an operation
            // such as CountAsync() or ToListAsync() is called.
            var query = _context.Employees
                .AsNoTracking()
                .AsQueryable();

            // STEP 1: FILTERING
            //
            // Apply a department filter only when a non-empty department
            // value has been provided.
            if (!string.IsNullOrWhiteSpace(department))
            {
                // Keep only employees belonging to the requested department.
                query = query.Where(e => e.Department == department);
            }

            // STEP 2: COUNT MATCHING RECORDS
            //
            // Count after filtering but before pagination.
            // This gives the total number of matching employees,
            // not just the number displayed on the current page.
            var totalRecords = await query.CountAsync();

            // STEP 3: DETERMINE SORT DIRECTION
            //
            // If sortOrder is "desc", use descending order.
            // Ignore capitalization, so "DESC" and "desc" both work.
            var desc = string.Equals(
                sortOrder,
                "desc",
                StringComparison.OrdinalIgnoreCase);

            // STEP 4: SORTING
            //
            // Select the sorting column requested by the caller.
            // ToLowerInvariant() makes the sortBy comparison case-insensitive.
            //
            // ThenBy(e => e.Id) provides a consistent secondary ordering
            // when multiple employees have the same primary sort value.
            query = sortBy?.ToLowerInvariant() switch
            {
                // Sort by employee name.
                "name" => desc
                    ? query.OrderByDescending(e => e.Name).ThenBy(e => e.Id)
                    : query.OrderBy(e => e.Name).ThenBy(e => e.Id),

                // Sort by employee salary.
                "salary" => desc
                    ? query.OrderByDescending(e => e.Salary).ThenBy(e => e.Id)
                    : query.OrderBy(e => e.Salary).ThenBy(e => e.Id),

                // Sort by department name.
                "department" => desc
                    ? query.OrderByDescending(e => e.Department).ThenBy(e => e.Id)
                    : query.OrderBy(e => e.Department).ThenBy(e => e.Id),

                // If sortBy is missing or unsupported, sort by employee ID.
                _ => query.OrderBy(e => e.Id)
            };

            // STEP 5: PAGINATION
            //
            // Skip records belonging to previous pages.
            // Take only the number of records requested for this page.
            //
            // Example:
            // page = 2, pageSize = 10
            // Skip((2 - 1) * 10) = Skip(10)
            // Take(10) returns the next 10 employees.
            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return both the current page of employees and the total count.
            // These are named tuple elements: Employees and TotalRecords.
            return (employees, totalRecords);
        }

        // Finds an employee by primary key (Id).
        //
        // Returns the Employee if found; otherwise, returns null.
        public async Task<Employee?> GetByIdAsync(int id)
        {
            // FindAsync searches for the entity using its primary key.
            return await _context.Employees.FindAsync(id);
        }

        // Adds a new employee to EF Core's change tracker.
        //
        // This method does not itself guarantee that the record is
        // saved to the database. SaveChangesAsync() persists the change.
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        // Marks the employee entity as modified in EF Core.
        //
        // The database update is persisted when SaveChangesAsync() runs.
        public Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);

            // The EF Core Update() method is synchronous.
            // Return a completed Task to match this method's Task-based signature.
            return Task.CompletedTask;
        }

        // Marks the employee entity for deletion.
        //
        // The actual database deletion happens when SaveChangesAsync() runs.
        public Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);

            // Remove() is synchronous, so no actual asynchronous work
            // needs to be awaited in this method.
            return Task.CompletedTask;
        }

        // Persists pending tracked changes to the database.
        //
        // This is where pending Add, Update, and Remove operations
        // are normally sent to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
