
// ============================================================
// REQUIRED NAMESPACE
// ============================================================

// Provides access to the Employee entity/model.
//
// We need Employee here because the repository interface
// defines methods that accept and return Employee objects.
using EmployeeManagement.API.Models;


// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains repository-related classes
// and interfaces.
//
// The Repository layer is responsible for defining and
// implementing database access operations.
namespace EmployeeManagement.API.Repositories
{
    // ========================================================
    // EMPLOYEE REPOSITORY INTERFACE
    // ========================================================

    // IEmployeeRepository defines the contract for
    // EmployeeRepository.
    //
    // It tells us WHAT database operations are available,
    // but it does not contain the actual implementation.
    //
    // The implementation is provided by:
    //
    // EmployeeRepository : IEmployeeRepository
    //
    // This is useful because the Service layer depends on
    // the interface instead of directly depending on the
    // concrete repository class.
    //
    // Architecture:
    //
    // EmployeeService
    //       ↓
    // IEmployeeRepository
    //       ↓
    // EmployeeRepository
    //
    // This also makes Dependency Injection and Unit Testing
    // easier.
    public interface IEmployeeRepository
    {
        // ====================================================
        // GET PAGINATED EMPLOYEES
        // ====================================================

        // Retrieves employees using:
        //
        // 1. Pagination
        // 2. Department filtering
        // 3. Sorting
        //
        // Parameters:
        //
        // page
        //     → Which page the client wants.
        //
        // pageSize
        //     → Number of employees per page.
        //
        // department
        //     → Optional department filter.
        //
        // sortBy
        //     → Field used for sorting.
        //        Example: name, salary, department
        //
        // sortOrder
        //     → Sorting direction.
        //        Example: asc or desc
        //
        // Return value:
        //
        // List<Employee> Employees
        //     → Employees for the requested page.
        //
        // int TotalRecords
        //     → Total number of matching records before
        //       pagination is applied.
        //
        // Example:
        //
        // Database contains 100 employees.
        //
        // page = 2
        // pageSize = 10
        //
        // Employees → employees 11 to 20
        // TotalRecords → 100
        //
        // The actual implementation is inside
        // EmployeeRepository.GetPagedAsync().
        Task<(List<Employee> Employees, int TotalRecords)> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder);


        // ====================================================
        // GET EMPLOYEE BY ID
        // ====================================================

        // Retrieves one employee using the employee ID.
        //
        // Example:
        //
        // GET /api/employees/5
        //
        // id = 5
        //
        // If employee 5 exists:
        //
        //     Employee object is returned.
        //
        // If employee 5 does not exist:
        //
        //     null is returned.
        //
        // Employee? means the result is allowed to be null.
        Task<Employee?> GetByIdAsync(int id);


        // ====================================================
        // ADD EMPLOYEE
        // ====================================================

        // Adds a new Employee entity to the EF Core context.
        //
        // This method prepares the entity to be inserted.
        //
        // Important:
        //
        // AddAsync() itself does not permanently save the
        // record to SQL Server.
        //
        // SaveChangesAsync() is required to actually persist
        // the INSERT operation to the database.
        //
        // Flow:
        //
        // AddAsync(employee)
        //        ↓
        // EF Core tracks entity as Added
        //        ↓
        // SaveChangesAsync()
        //        ↓
        // INSERT into Employees
        Task AddAsync(Employee employee);


        // ====================================================
        // UPDATE EMPLOYEE
        // ====================================================

        // Updates an existing employee.
        //
        // The Employee object contains the updated values.
        //
        // The repository implementation marks the entity as
        // Modified using EF Core.
        //
        // The actual UPDATE SQL command is sent to the database
        // when SaveChangesAsync() is called.
        Task UpdateAsync(Employee employee);


        // ====================================================
        // DELETE EMPLOYEE
        // ====================================================

        // Deletes an existing employee.
        //
        // The Employee object represents the record that
        // should be removed.
        //
        // The repository implementation marks the entity
        // as Deleted using EF Core.
        //
        // The actual DELETE SQL command is executed when
        // SaveChangesAsync() is called.
        Task DeleteAsync(Employee employee);


        // ====================================================
        // SAVE CHANGES
        // ====================================================

        // Saves all pending changes tracked by Entity
        // Framework Core to the database.
        //
        // Depending on the entity state, EF Core can execute:
        //
        // Added    → INSERT
        // Modified → UPDATE
        // Deleted  → DELETE
        //
        // This method is kept separate from Add/Update/Delete
        // so the Service layer can control when the changes
        // are actually committed.
        Task SaveChangesAsync();
    }
}

