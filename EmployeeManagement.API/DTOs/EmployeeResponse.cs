
// ============================================================
// NAMESPACE
// ============================================================
//
// DTOs are kept in a separate folder/namespace because they
// represent data transferred between the client and API.
//
// DTO = Data Transfer Object.
//
// In this case, EmployeeResponse represents the data that
// our API sends BACK to the client.
//
// ============================================================

namespace EmployeeManagement.API.DTOs
{
    // ========================================================
    // EMPLOYEE RESPONSE DTO
    // ========================================================

    // EmployeeResponse is an OUTPUT DTO.
    //
    // It defines what employee information the API should
    // return to the client.
    //
    // For example, when the client calls:
    //
    // GET /api/employees/1
    //
    // instead of directly returning the Employee database
    // entity, we can return an EmployeeResponse object.
    //
    // Flow:
    //
    // Database
    //     ↓
    // Employee Entity
    //     ↓
    // Mapping
    //     ↓
    // EmployeeResponse DTO
    //     ↓
    // JSON
    //     ↓
    // Client
    //
    public class EmployeeResponse
    {
        // ====================================================
        // EMPLOYEE ID
        // ====================================================

        // Unique identifier of the employee.
        //
        // Example:
        //
        // Id = 101
        //
        // This is generally returned so that the client
        // knows which employee/resource it is working with.
        public int Id { get; set; }


        // ====================================================
        // EMPLOYEE NAME
        // ====================================================

        // Employee's name.
        //
        // = string.Empty initializes the property with an
        // empty string instead of null.
        //
        // This helps avoid nullable reference type warnings
        // and gives the property a default value.
        public string Name { get; set; } = string.Empty;


        // ====================================================
        // EMPLOYEE EMAIL
        // ====================================================

        // Employee's email address.
        //
        // This property is included in the API response
        // so the client can display/use the employee email.
        public string Email { get; set; } = string.Empty;


        // ====================================================
        // DEPARTMENT
        // ====================================================

        // Employee's department.
        //
        // Example:
        //
        // IT
        // HR
        // Finance
        // Operations
        //
        public string Department { get; set; } = string.Empty;


        // ====================================================
        // SALARY
        // ====================================================

        // Employee salary.
        //
        // decimal is used because salary is a monetary value
        // and decimal provides suitable precision for
        // financial calculations.
        //
        // Example:
        //
        // 75000.50
        //
        public decimal Salary { get; set; }
    }
}

