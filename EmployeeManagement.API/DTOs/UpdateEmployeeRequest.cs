
// ============================================================
// IMPORT / NAMESPACE
// ============================================================

// System.ComponentModel.DataAnnotations provides validation
// attributes such as:
// [Required]
// [StringLength]
// [EmailAddress]
// [Range]
//
// These attributes allow ASP.NET Core to validate the incoming
// request before it reaches the business logic.
using System.ComponentModel.DataAnnotations;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.DTOs
{
    // ========================================================
    // UPDATE EMPLOYEE REQUEST DTO
    // ========================================================

    // UpdateEmployeeRequest is an INPUT DTO.
    //
    // It is used when the client wants to update an existing
    // employee.
    //
    // Example API:
    //
    // PUT /api/employees/10
    //
    // Request body:
    //
    // {
    //     "name": "Ashish Shinde",
    //     "email": "ashish@example.com",
    //     "department": "IT",
    //     "salary": 85000
    // }
    //
    // The employee ID is NOT part of this DTO because the ID
    // is normally provided through the URL:
    //
    // PUT /api/employees/{id}
    //
    // Example:
    //
    // PUT /api/employees/10
    //
    // Here:
    //
    // id = 10
    //
    // and the request body contains the values that need to
    // be updated.
    //
    public class UpdateEmployeeRequest
    {
        // ====================================================
        // EMPLOYEE NAME
        // ====================================================

        // [Required]
        //
        // Makes sure that the client provides a Name.
        //
        // If Name is missing or invalid, model validation
        // fails.
        //
        // Because the controller has [ApiController],
        // ASP.NET Core can automatically return:
        //
        // 400 Bad Request
        //
        [Required]


        // [StringLength(100)]
        //
        // Limits the employee name to a maximum of
        // 100 characters.
        //
        // Example:
        //
        // "Ashish Shinde"       → Valid
        // Very long name > 100  → Invalid
        //
        [StringLength(100)]


        // Contains the new employee name that should be saved.
        //
        // = string.Empty initializes the property with an
        // empty string instead of null.
        //
        // IMPORTANT:
        //
        // string.Empty itself does NOT perform validation.
        // [Required] performs the validation.
        public string Name { get; set; } = string.Empty;


        // ====================================================
        // EMAIL
        // ====================================================

        // [Required]
        //
        // Email must be provided when updating the employee.
        [Required]


        // [EmailAddress]
        //
        // Validates that the value looks like an email address.
        //
        // Example:
        //
        // "ashish@example.com" → Valid format
        //
        // "ashish"             → Invalid format
        //
        // This is basic format validation. It does not prove
        // that the email address actually exists.
        [EmailAddress]


        // Contains the new email address of the employee.
        public string Email { get; set; } = string.Empty;


        // ====================================================
        // DEPARTMENT
        // ====================================================

        // [Required]
        //
        // Department must be provided during the update.
        [Required]


        // [StringLength(100)]
        //
        // Department can contain a maximum of 100 characters.
        [StringLength(100)]


        // Contains the employee's new department.
        //
        // Example:
        //
        // "IT"
        // "HR"
        // "Finance"
        // "Operations"
        //
        public string Department { get; set; } = string.Empty;


        // ====================================================
        // SALARY
        // ====================================================

        // [Range(1, 100000000)]
        //
        // Validates that Salary is between:
        //
        // Minimum = 1
        // Maximum = 100,000,000
        //
        // Examples:
        //
        // Salary = 50000      → Valid
        // Salary = 100000     → Valid
        // Salary = 0          → Invalid
        // Salary > 100000000  → Invalid
        //
        // decimal is used for salary because salary is a
        // financial/monetary value and decimal provides
        // appropriate precision for such values.
        [Range(1, 100000000)]


        // Contains the new salary value for the employee.
        //
        // Example:
        //
        // 75000
        // 85000.50
        //
        public decimal Salary { get; set; }
    }
}

