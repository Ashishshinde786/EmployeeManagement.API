
// ============================================================
// REQUIRED NAMESPACE
// ============================================================

// Provides access to DTO classes.
//
// This interface uses DTOs for communication between the
// Controller layer and the Service layer.
//
// DTOs used here:
// - PagedEmployeeResponse
// - EmployeeResponse
// - CreateEmployeeRequest
// - UpdateEmployeeRequest
using EmployeeManagement.API.DTOs;


// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains service classes and interfaces.
//
// The Service layer is responsible for application/business
// logic.
//
// Architecture:
//
// EmployeeController
//        ↓
// IEmployeeService
//        ↓
// EmployeeService
//        ↓
// IEmployeeRepository
//        ↓
// EmployeeRepository
//        ↓
// AppDbContext
//        ↓
// SQL Server
namespace EmployeeManagement.API.Services
{
    // ========================================================
    // EMPLOYEE SERVICE INTERFACE
    // ========================================================

    // IEmployeeService defines the contract for employee-related
    // business operations.
    //
    // It tells the application WHAT operations are available,
    // but it does not contain the actual implementation.
    //
    // The implementation is provided by:
    //
    // EmployeeService : IEmployeeService
    //
    // Why use an interface?
    //
    // - Loose coupling
    // - Dependency Injection
    // - Easier unit testing
    // - Separation of responsibilities
    //
    // The Controller depends on this abstraction instead of
    // directly depending on EmployeeService.
    public interface IEmployeeService
    {
        // ====================================================
        // GET PAGINATED EMPLOYEES
        // ====================================================

        // Retrieves employees with:
        //
        // - Pagination
        // - Department filtering
        // - Sorting
        //
        // Parameters:
        //
        // page
        //     → Requested page number.
        //
        // pageSize
        //     → Number of employees per page.
        //
        // department
        //     → Optional department filter.
        //
        // sortBy
        //     → Field by which employees should be sorted.
        //        Example: name, salary, department.
        //
        // sortOrder
        //     → Sorting direction.
        //        Example: asc or desc.
        //
        // The Service implementation calls the repository
        // to retrieve the data and then creates a
        // PagedEmployeeResponse.
        //
        // The response contains:
        //
        // - Employees
        // - Page
        // - PageSize
        // - TotalRecords
        // - TotalPages
        Task<PagedEmployeeResponse> GetPagedAsync(
            int page,
            int pageSize,
            string? department,
            string? sortBy,
            string? sortOrder);


        // ====================================================
        // GET EMPLOYEE BY ID
        // ====================================================

        // Retrieves a single employee using the employee ID.
        //
        // Example:
        //
        // GET /api/employees/10
        //
        // If employee 10 exists:
        //
        //     EmployeeResponse is returned.
        //
        // If employee 10 does not exist:
        //
        //     null is returned.
        //
        // EmployeeResponse is used instead of returning the
        // Employee entity directly.
        //
        // This keeps the API contract separate from the
        // database entity.
        Task<EmployeeResponse?> GetByIdAsync(int id);


        // ====================================================
        // CREATE EMPLOYEE
        // ====================================================

        // Creates a new employee.
        //
        // CreateEmployeeRequest contains the data received
        // from the API client.
        //
        // Example request:
        //
        // {
        //     "name": "Ashish",
        //     "email": "ashish@example.com",
        //     "department": "IT",
        //     "salary": 80000
        // }
        //
        // The Service implementation:
        //
        // 1. Converts the DTO into an Employee entity.
        // 2. Sends the entity to the repository.
        // 3. Saves the changes to the database.
        // 4. Converts the created entity into
        //    EmployeeResponse.
        //
        // EmployeeResponse is returned after successful
        // creation.
        Task<EmployeeResponse> CreateAsync(
            CreateEmployeeRequest request);


        // ====================================================
        // UPDATE EMPLOYEE
        // ====================================================

        // Updates an existing employee.
        //
        // id:
        //     → ID of the employee that should be updated.
        //
        // request:
        //     → Contains the new employee information.
        //
        // Example:
        //
        // PUT /api/employees/10
        //
        // The Service implementation:
        //
        // 1. Finds the existing employee.
        // 2. Checks whether the employee exists.
        // 3. Updates the entity properties.
        // 4. Saves the changes.
        //
        // Return value:
        //
        // true
        //     → Employee was found and updated.
        //
        // false
        //     → Employee was not found.
        //
        // The Controller uses this boolean result to decide
        // whether to return a successful response or
        // HTTP 404 Not Found.
        Task<bool> UpdateAsync(
            int id,
            UpdateEmployeeRequest request);


        // ====================================================
        // DELETE EMPLOYEE
        // ====================================================

        // Deletes an employee using the employee ID.
        //
        // Example:
        //
        // DELETE /api/employees/10
        //
        // The Service implementation:
        //
        // 1. Finds the employee.
        // 2. Checks whether the employee exists.
        // 3. Asks the repository to delete it.
        // 4. Saves the changes to the database.
        //
        // Return value:
        //
        // true
        //     → Employee was found and deleted.
        //
        // false
        //     → Employee was not found.
        //
        // The Controller can use this result to return:
        //
        // 204 No Content
        //
        // or:
        //
        // 404 Not Found
        Task<bool> DeleteAsync(int id);
    }
}

