
// ============================================================
// REQUIRED NAMESPACES
// ============================================================

// Provides access to DTO classes.
//
// DTOs are used to transfer data between the Controller/API
// and the Service layer.
//
// Examples:
// - CreateEmployeeRequest
// - UpdateEmployeeRequest
// - EmployeeResponse
// - PagedEmployeeResponse
using EmployeeManagement.API.DTOs;


// Provides access to the Employee entity/model.
//
// The Employee entity represents the employee data stored
// in the database.
using EmployeeManagement.API.Models;


// Provides access to the repository interface.
//
// EmployeeService communicates with the database through
// IEmployeeRepository instead of directly using AppDbContext.
//
// This keeps the Service layer separated from the
// database-access logic.
using EmployeeManagement.API.Repositories;


// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains service classes.
//
// The Service layer contains application/business logic.
//
// Architecture:
//
// Controller
//      ↓
// EmployeeService
//      ↓
// IEmployeeRepository
//      ↓
// EmployeeRepository
//      ↓
// AppDbContext
//      ↓
// SQL Server
namespace EmployeeManagement.API.Services
{
    // ========================================================
    // EMPLOYEE SERVICE
    // ========================================================

    // EmployeeService implements IEmployeeService.
    //
    // The interface defines WHAT operations the service
    // provides, while this class contains the actual
    // implementation.
    //
    // Main responsibilities of this service:
    //
    // - Get employees
    // - Apply pagination/filtering/sorting through repository
    // - Convert Entity objects into DTOs
    // - Create employees
    // - Update employees
    // - Delete employees
    // - Log important application events
    //
    // The service does NOT directly access SQL Server.
    // It uses IEmployeeRepository for database operations.
    public class EmployeeService : IEmployeeService
    {
        // ====================================================
        // EMPLOYEE REPOSITORY
        // ====================================================

        // _repository represents the repository dependency.
        //
        // The Service layer uses this interface to communicate
        // with the database layer.
        //
        // We depend on IEmployeeRepository instead of the
        // concrete EmployeeRepository class.
        //
        // This provides:
        //
        // - Loose coupling
        // - Dependency Injection
        // - Easier unit testing
        private readonly IEmployeeRepository _repository;


        // ====================================================
        // LOGGER
        // ====================================================

        // ILogger<EmployeeService> is used for application
        // logging.
        //
        // We use logs to understand what the application is
        // doing and to help troubleshoot issues.
        //
        // Examples:
        //
        // Information → normal application activity
        // Warning     → something unexpected but handled
        // Error       → serious failure
        private readonly ILogger<EmployeeService> _logger;


        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // Dependencies are injected through the constructor.
        //
        // ASP.NET Core Dependency Injection creates these
        // objects and provides them to EmployeeService.
        public EmployeeService(
            IEmployeeRepository repository,
            ILogger<EmployeeService> logger)
        {
            // Store the repository dependency so all service
            // methods can use it.
            _repository = repository;

            // Store the logger dependency so all service
            // methods can write logs.
            _logger = logger;
        }


        // ============================================================
        // GET ALL EMPLOYEES
        // PAGINATION + FILTERING + SORTING
        // ============================================================

        // Retrieves a paginated list of employees.
        //
        // The method supports:
        //
        // - Pagination
        // - Department filtering
        // - Sorting
        //
        // The repository performs the database query.
        //
        // The service then converts Employee entities into
        // EmployeeResponse DTOs and creates the final
        // PagedEmployeeResponse.
        public async Task<PagedEmployeeResponse> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder)
        {
            // =================================================
            // LOG REQUEST INFORMATION
            // =================================================

            // Write an Information-level log before fetching
            // employees.
            //
            // Structured logging is used here.
            //
            // Instead of building a string manually, values are
            // provided as parameters:
            //
            // {Page}
            // {PageSize}
            // {Department}
            // {SortBy}
            // {SortOrder}
            //
            // Logging frameworks can store these values
            // separately and make them searchable.
            _logger.LogInformation(
                "Fetching employees. Page: {Page}, PageSize: {PageSize}, Department: {Department}, SortBy: {SortBy}, SortOrder: {SortOrder}",
                page,
                pageSize,
                department,
                sortBy,
                sortOrder);


            // =================================================
            // GET DATA FROM REPOSITORY
            // =================================================

            // Call the repository to retrieve the employee
            // data.
            //
            // The repository handles:
            //
            // - Filtering
            // - Sorting
            // - Pagination
            // - Database communication
            //
            // The result contains:
            //
            // result.Employees
            //     → Employees for the requested page
            //
            // result.TotalRecords
            //     → Total number of matching records
            var result = await _repository.GetPagedAsync(
                page,
                pageSize,
                department,
                sortBy,
                sortOrder);


            // =================================================
            // MAP ENTITY TO DTO
            // =================================================

            // Convert Employee entities into
            // EmployeeResponse DTOs.
            //
            // Why do we need DTOs?
            //
            // We should not directly expose database entities
            // through our API.
            //
            // DTOs allow us to control exactly which fields
            // are returned to the client.
            //
            // LINQ Select() converts every Employee object
            // into an EmployeeResponse object.
            var employees = result.Employees
                .Select(employee => new EmployeeResponse
                {
                    // Copy Employee ID to response DTO.
                    Id = employee.Id,

                    // Copy employee name.
                    Name = employee.Name,

                    // Copy employee email.
                    Email = employee.Email,

                    // Copy department.
                    Department = employee.Department,

                    // Copy salary.
                    Salary = employee.Salary
                })
                .ToList();


            // =================================================
            // CALCULATE TOTAL PAGES
            // =================================================

            // Calculate how many pages are required to display
            // all matching records.
            //
            // Formula:
            //
            // TotalPages =
            //     Ceiling(TotalRecords / PageSize)
            //
            // Example:
            //
            // TotalRecords = 25
            // PageSize = 10
            //
            // 25 / 10 = 2.5
            //
            // Ceiling(2.5) = 3 pages
            //
            // Math.Ceiling() is used because even if the final
            // page contains only a few records, it still counts
            // as a complete page.
            var totalPages =
                (int)Math.Ceiling(
                    (double)result.TotalRecords / pageSize);


            // =================================================
            // CREATE PAGED RESPONSE
            // =================================================

            // Create the final response object that will be
            // returned to the Controller.
            //
            // This response contains both:
            //
            // - Actual employee data
            // - Pagination metadata
            return new PagedEmployeeResponse
            {
                // Employees for the current page.
                Employees = employees,

                // Current page number.
                Page = page,

                // Number of records requested per page.
                PageSize = pageSize,

                // Total matching records.
                TotalRecords = result.TotalRecords,

                // Total number of pages.
                TotalPages = totalPages
            };
        }


        // ============================================================
        // GET EMPLOYEE BY ID
        // ============================================================

        // Retrieves one employee using the employee ID.
        //
        // Example:
        //
        // GET /api/employees/10
        //
        // If employee 10 exists:
        //     EmployeeResponse is returned.
        //
        // If employee 10 does not exist:
        //     null is returned.
        public async Task<EmployeeResponse?> GetByIdAsync(int id)
        {
            // Log that an employee lookup has started.
            _logger.LogInformation(
                "Fetching employee with ID {EmployeeId}.",
                id);


            // =================================================
            // GET EMPLOYEE FROM REPOSITORY
            // =================================================

            // Ask the repository to find the employee.
            //
            // The repository handles the actual database query.
            var employee =
                await _repository.GetByIdAsync(id);


            // =================================================
            // EMPLOYEE NOT FOUND
            // =================================================

            // If the repository returns null, the employee
            // does not exist.
            if (employee == null)
            {
                // Log a warning because the requested employee
                // was not found.
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found.",
                    id);

                // Return null to the Controller.
                //
                // The Controller converts this into:
                //
                // HTTP 404 Not Found
                return null;
            }


            // =================================================
            // MAP ENTITY TO DTO
            // =================================================

            // Convert the database Employee entity into an
            // EmployeeResponse DTO.
            //
            // Again, the API returns a DTO instead of directly
            // exposing the database entity.
            return new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Salary = employee.Salary
            };
        }


        // ============================================================
        // CREATE EMPLOYEE
        // ============================================================

        // Creates a new employee.
        //
        // The Controller sends a CreateEmployeeRequest DTO.
        //
        // The Service converts the DTO into an Employee entity
        // and sends it to the Repository.
        public async Task<EmployeeResponse> CreateAsync(
            CreateEmployeeRequest request)
        {
            // =================================================
            // CREATE ENTITY FROM DTO
            // =================================================

            // Convert the incoming CreateEmployeeRequest DTO
            // into an Employee entity.
            //
            // The DTO represents API input.
            //
            // The Entity represents database data.
            var employee = new Employee
            {
                // Copy name from request to entity.
                Name = request.Name,

                // Copy email from request to entity.
                Email = request.Email,

                // Copy department from request to entity.
                Department = request.Department,

                // Copy salary from request to entity.
                Salary = request.Salary
            };


            // =================================================
            // ADD EMPLOYEE
            // =================================================

            // Ask the repository to add the employee.
            //
            // The repository marks the entity as Added in
            // EF Core.
            await _repository.AddAsync(employee);


            // =================================================
            // SAVE CHANGES
            // =================================================

            // Persist the new employee to the database.
            //
            // This causes EF Core to execute the INSERT.
            //
            // After SaveChangesAsync(), the generated database
            // ID is available in employee.Id.
            await _repository.SaveChangesAsync();


            // =================================================
            // LOG SUCCESS
            // =================================================

            // Log that the employee was successfully created.
            //
            // employee.Id contains the generated primary key.
            _logger.LogInformation(
                "Employee created successfully with ID {EmployeeId}.",
                employee.Id);


            // =================================================
            // RETURN RESPONSE DTO
            // =================================================

            // Convert the newly created Employee entity into
            // an EmployeeResponse DTO.
            //
            // The DTO is returned to the Controller and then
            // sent to the API client.
            return new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Salary = employee.Salary
            };
        }


        // ============================================================
        // UPDATE EMPLOYEE
        // ============================================================

        // Updates an existing employee.
        //
        // Parameters:
        //
        // id
        //     → Employee ID from the URL.
        //
        // request
        //     → New employee data from the request body.
        //
        // Returns:
        //
        // true
        //     → Employee was updated.
        //
        // false
        //     → Employee was not found.
        public async Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request)
        {
            // Log that an update operation has started.
            _logger.LogInformation(
                "Updating employee with ID {EmployeeId}.",
                id);


            // =================================================
            // FIND EXISTING EMPLOYEE
            // =================================================

            // First retrieve the employee from the database.
            //
            // We need the existing entity before changing its
            // properties.
            var existingEmployee =
                await _repository.GetByIdAsync(id);


            // =================================================
            // EMPLOYEE NOT FOUND
            // =================================================

            // If no employee exists with this ID, the update
            // cannot be performed.
            if (existingEmployee == null)
            {
                // Log a warning for troubleshooting.
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found for update.",
                    id);

                // Return false to tell the Controller that
                // the employee does not exist.
                //
                // Controller will return:
                //
                // HTTP 404 Not Found
                return false;
            }


            // =================================================
            // UPDATE ENTITY PROPERTIES
            // =================================================

            // Update the existing entity with the values
            // received from the request.
            existingEmployee.Name = request.Name;

            // Update employee email.
            existingEmployee.Email = request.Email;

            // Update department.
            existingEmployee.Department = request.Department;

            // Update salary.
            existingEmployee.Salary = request.Salary;


            // =================================================
            // MARK ENTITY AS UPDATED
            // =================================================

            // Tell the repository that the employee should
            // be updated.
            //
            // The repository uses EF Core's Update() method
            // to mark the entity as Modified.
            await _repository.UpdateAsync(existingEmployee);


            // =================================================
            // SAVE CHANGES
            // =================================================

            // Persist the UPDATE operation to SQL Server.
            await _repository.SaveChangesAsync();


            // =================================================
            // LOG SUCCESS
            // =================================================

            // Log successful update.
            _logger.LogInformation(
                "Employee with ID {EmployeeId} updated successfully.",
                id);


            // Return true to indicate that the update succeeded.
            return true;
        }


        // ============================================================
        // DELETE EMPLOYEE
        // ============================================================

        // Deletes an employee using the employee ID.
        //
        // Returns:
        //
        // true
        //     → Employee was deleted.
        //
        // false
        //     → Employee was not found.
        public async Task<bool> DeleteAsync(int id)
        {
            // Log that the delete operation has started.
            _logger.LogInformation(
                "Deleting employee with ID {EmployeeId}.",
                id);


            // =================================================
            // FIND EMPLOYEE
            // =================================================

            // First find the employee that should be deleted.
            var employee =
                await _repository.GetByIdAsync(id);


            // =================================================
            // EMPLOYEE NOT FOUND
            // =================================================

            // If the employee does not exist, there is nothing
            // to delete.
            if (employee == null)
            {
                // Log a warning for troubleshooting.
                _logger.LogWarning(
                    "Employee with ID {EmployeeId} was not found for deletion.",
                    id);

                // Return false.
                //
                // Controller will return:
                //
                // HTTP 404 Not Found
                return false;
            }


            // =================================================
            // DELETE EMPLOYEE
            // =================================================

            // Ask the repository to remove the employee.
            //
            // Repository marks the entity as Deleted in EF Core.
            await _repository.DeleteAsync(employee);


            // =================================================
            // SAVE CHANGES
            // =================================================

            // Persist the DELETE operation to SQL Server.
            await _repository.SaveChangesAsync();


            // =================================================
            // LOG SUCCESS
            // =================================================

            // Log successful deletion.
            _logger.LogInformation(
                "Employee with ID {EmployeeId} deleted successfully.",
                id);


            // Return true to indicate successful deletion.
            return true;
        }
    }
}

