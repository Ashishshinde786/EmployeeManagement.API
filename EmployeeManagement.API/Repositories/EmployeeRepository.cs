
// ============================================================
// REQUIRED NAMESPACES
// ============================================================

// Provides access to AppDbContext.
//
// AppDbContext is our EF Core database context that connects
// the application with the SQL Server database.
using EmployeeManagement.API.Data;


// Provides access to the Employee entity/model.
//
// EmployeeRepository works with Employee objects when
// reading, creating, updating, and deleting employee data.
using EmployeeManagement.API.Models;


// Provides Entity Framework Core functionality.
//
// We use EF Core methods such as:
//
// - AsQueryable()
// - Where()
// - CountAsync()
// - OrderBy()
// - OrderByDescending()
// - Skip()
// - Take()
// - ToListAsync()
// - FindAsync()
// - AddAsync()
// - SaveChangesAsync()
//
// These methods allow us to interact with the database
// using C# and LINQ.
using Microsoft.EntityFrameworkCore;


// ============================================================
// NAMESPACE
// ============================================================

// Repository classes are placed inside the Repositories
// namespace.
//
// The repository layer is responsible for database access.
//
// Architecture:
//
// Controller
//     ↓
// Service
//     ↓
// Repository
//     ↓
// AppDbContext
//     ↓
// SQL Server
namespace EmployeeManagement.API.Repositories
{
    // ========================================================
    // EMPLOYEE REPOSITORY
    // ========================================================

    // EmployeeRepository implements IEmployeeRepository.
    //
    // IEmployeeRepository defines the database operations
    // that the EmployeeRepository must provide.
    //
    // This class contains the actual implementation of those
    // database operations.
    //
    // Main responsibilities:
    //
    // - Get employees
    // - Filter employees
    // - Sort employees
    // - Paginate employees
    // - Get employee by ID
    // - Add employee
    // - Update employee
    // - Delete employee
    // - Save changes to database
    public class EmployeeRepository : IEmployeeRepository
    {
        // ====================================================
        // DATABASE CONTEXT
        // ====================================================

        // _context represents our EF Core database context.
        //
        // AppDbContext provides access to database tables
        // through DbSet properties.
        //
        // In our AppDbContext we have:
        //
        // DbSet<Employee> Employees
        //
        // Therefore we can access the Employees table using:
        //
        // _context.Employees
        //
        // readonly means that after the constructor assigns
        // the context, this field cannot be reassigned.
        private readonly AppDbContext _context;


        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // The constructor receives AppDbContext through
        // Dependency Injection.
        //
        // Program.cs contains the registration:
        //
        // AddDbContext<AppDbContext>(...)
        //
        // ASP.NET Core creates the AppDbContext and injects
        // it into this repository.
        public EmployeeRepository(AppDbContext context)
        {
            // Store the injected database context in the
            // private field so all repository methods can
            // use it.
            _context = context;
        }


        // ====================================================
        // GET ALL EMPLOYEES
        // FILTERING + SORTING + PAGINATION
        // ====================================================

        // This method retrieves employees from the database
        // using:
        //
        // 1. Filtering
        // 2. Sorting
        // 3. Pagination
        //
        // It returns two values:
        //
        // Employees
        //     → Employees for the requested page
        //
        // TotalRecords
        //     → Total number of records after filtering
        //
        // The return type is a named tuple:
        //
        // (
        //     List<Employee> Employees,
        //     int TotalRecords
        // )
        //
        // async/await is used because database operations are
        // I/O operations and should not block the request thread.
        public async Task<(List<Employee> Employees, int TotalRecords)> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder)
        {
            // =================================================
            // START QUERY
            // =================================================

            // Start with the Employees table.
            //
            // AsQueryable() creates an IQueryable<Employee>.
            //
            // This is important because we can continue building
            // the query dynamically by adding:
            //
            // Where()
            // OrderBy()
            // Skip()
            // Take()
            //
            // EF Core can translate the final LINQ query into
            // SQL and execute it in the database.
            var query = _context.Employees.AsQueryable();


            // =================================================
            // 1. FILTERING
            // =================================================

            // Check whether a department filter was provided.
            //
            // string.IsNullOrWhiteSpace() returns true when:
            //
            // - department is null
            // - department is empty
            // - department contains only spaces
            //
            // We only apply the filter when a meaningful
            // department value is provided.
            if (!string.IsNullOrWhiteSpace(department))
            {
                // Add a WHERE condition to the query.
                //
                // Only employees whose Department exactly
                // matches the requested department are selected.
                //
                // Example:
                //
                // department = "IT"
                //
                // Conceptually:
                //
                // SELECT *
                // FROM Employees
                // WHERE Department = 'IT'
                //
                query = query.Where(e =>
                    e.Department == department);
            }


            // =================================================
            // 2. COUNT FILTERED RECORDS
            // =================================================

            // Count the records after applying the department
            // filter.
            //
            // This value is required to calculate:
            //
            // TotalRecords
            //
            // Example:
            //
            // Database has 100 employees.
            //
            // Department = IT
            //
            // There are 35 IT employees.
            //
            // TotalRecords = 35
            //
            // CountAsync() executes the COUNT operation in the
            // database asynchronously.
            var totalRecords =
                await query.CountAsync();


            // =================================================
            // 3. SORTING
            // =================================================

            // Check whether the client requested sorting by
            // employee name.
            //
            // StringComparison.OrdinalIgnoreCase makes the
            // comparison case-insensitive.
            //
            // Therefore:
            //
            // "name"
            // "Name"
            // "NAME"
            //
            // are treated as the same value.
            if (string.Equals(
                sortBy,
                "name",
                StringComparison.OrdinalIgnoreCase))
            {
                // Check whether descending order was requested.
                //
                // If sortOrder = "desc":
                //
                // Z → A
                //
                // Otherwise:
                //
                // A → Z
                query = string.Equals(
                    sortOrder,
                    "desc",
                    StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Name)
                    : query.OrderBy(e => e.Name);
            }


            // =================================================
            // SORT BY SALARY
            // =================================================

            // If sortBy is "salary", sort employees according
            // to their salary.
            else if (string.Equals(
                sortBy,
                "salary",
                StringComparison.OrdinalIgnoreCase))
            {
                // Descending:
                //
                // Highest salary → Lowest salary
                //
                // Ascending:
                //
                // Lowest salary → Highest salary
                query = string.Equals(
                    sortOrder,
                    "desc",
                    StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Salary)
                    : query.OrderBy(e => e.Salary);
            }


            // =================================================
            // SORT BY DEPARTMENT
            // =================================================

            // If sortBy is "department", sort employees by
            // department name.
            else if (string.Equals(
                sortBy,
                "department",
                StringComparison.OrdinalIgnoreCase))
            {
                // Descending:
                //
                // Z → A
                //
                // Ascending:
                //
                // A → Z
                query = string.Equals(
                    sortOrder,
                    "desc",
                    StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Department)
                    : query.OrderBy(e => e.Department);
            }


            // =================================================
            // DEFAULT SORTING
            // =================================================

            // If the client does not provide a supported
            // sortBy value, we use Id as the default sorting.
            //
            // This gives the query a predictable order.
            else
            {
                query = query.OrderBy(e => e.Id);
            }


            // =================================================
            // 4. PAGINATION
            // =================================================

            // Skip() and Take() are used to implement
            // pagination.
            //
            // Formula:
            //
            // Skip = (page - 1) * pageSize
            //
            // Example:
            //
            // page = 1
            // pageSize = 10
            //
            // Skip = (1 - 1) * 10
            // Skip = 0
            //
            // So the first 10 records are returned.
            //
            // Example:
            //
            // page = 2
            // pageSize = 10
            //
            // Skip = (2 - 1) * 10
            // Skip = 10
            //
            // So records 1-10 are skipped and records 11-20
            // are returned.
            //
            // Take(pageSize) limits the number of records
            // returned for the current page.
            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            // =================================================
            // RETURN RESULT
            // =================================================

            // Return both:
            //
            // employees
            //     → Data for the requested page
            //
            // totalRecords
            //     → Total number of matching records
            //
            // The service layer uses these values to create
            // PagedEmployeeResponse.
            return (
                employees,
                totalRecords
            );
        }


        // ====================================================
        // GET EMPLOYEE BY ID
        // ====================================================

        // Retrieves a single employee using its ID.
        //
        // Example:
        //
        // GET /api/employees/10
        //
        // id = 10
        //
        // If the employee exists:
        //
        // Employee object is returned.
        //
        // If it does not exist:
        //
        // null is returned.
        public async Task<Employee?> GetByIdAsync(int id)
        {
            // FindAsync() searches for an entity using its
            // primary key.
            //
            // Since Employee.Id is the primary key, EF Core
            // searches for the employee with the specified ID.
            //
            // The ? in Employee? means the result can be null.
            return await _context.Employees
                .FindAsync(id);
        }


        // ====================================================
        // CREATE EMPLOYEE
        // ====================================================

        // Adds a new Employee entity to the EF Core context.
        //
        // Important:
        //
        // AddAsync() does not necessarily mean the record has
        // already been inserted into SQL Server.
        //
        // The actual database INSERT happens when:
        //
        // SaveChangesAsync()
        //
        // is called.
        public async Task AddAsync(Employee employee)
        {
            // Add the employee to EF Core's change tracker.
            //
            // EF Core marks this entity as Added.
            //
            // Later, SaveChangesAsync() generates the SQL INSERT.
            await _context.Employees
                .AddAsync(employee);
        }


        // ====================================================
        // UPDATE EMPLOYEE
        // ====================================================

        // Updates an existing employee entity.
        public Task UpdateAsync(Employee employee)
        {
            // Tell EF Core that this entity should be treated
            // as modified.
            //
            // EF Core tracks the entity as Modified.
            //
            // When SaveChangesAsync() is called, EF Core
            // generates the required SQL UPDATE statement.
            _context.Employees
                .Update(employee);

            // This method does not need to perform asynchronous
            // work itself.
            //
            // The actual database operation happens later
            // inside SaveChangesAsync().
            return Task.CompletedTask;
        }


        // ====================================================
        // DELETE EMPLOYEE
        // ====================================================

        // Marks an employee for deletion.
        public Task DeleteAsync(Employee employee)
        {
            // Tell EF Core that the employee should be deleted.
            //
            // EF Core marks the entity as Deleted.
            //
            // The actual DELETE SQL command is executed when
            // SaveChangesAsync() is called.
            _context.Employees
                .Remove(employee);

            // No asynchronous database operation is performed
            // at this point.
            //
            // The actual database operation happens in
            // SaveChangesAsync().
            return Task.CompletedTask;
        }


        // ====================================================
        // SAVE CHANGES
        // ====================================================

        // Persists all pending changes tracked by EF Core
        // to the database.
        //
        // This is where EF Core actually sends INSERT, UPDATE,
        // and DELETE commands to SQL Server.
        public async Task SaveChangesAsync()
        {
            // Save all pending changes asynchronously.
            //
            // Examples:
            //
            // Added entity    → INSERT
            // Modified entity → UPDATE
            // Deleted entity  → DELETE
            await _context.SaveChangesAsync();
        }
    }
}

