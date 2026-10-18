
// ============================================================
// NAMESPACE / IMPORT
// ============================================================

// System.ComponentModel.DataAnnotations contains
// validation attributes such as:
//
// [Required]
// [StringLength]
// [EmailAddress]
// [Range]
//
// These attributes allow us to define validation rules
// directly on our DTO properties.
using System.ComponentModel.DataAnnotations;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.DTOs
{
    // ========================================================
    // CREATE EMPLOYEE REQUEST DTO
    // ========================================================

    // DTO = Data Transfer Object.
    //
    // This class represents the data that the client is
    // allowed/expected to send when creating an employee.
    //
    // It is NOT our database entity.
    //
    // Client
    //    ↓
    // CreateEmployeeRequest DTO
    //    ↓
    // EmployeeService
    //    ↓
    // Employee Entity
    //    ↓
    // EF Core
    //    ↓
    // SQL Server
    //
    public class CreateEmployeeRequest
    {
        // ====================================================
        // EMPLOYEE NAME
        // ====================================================

        // [Required] means this property must have a value.
        //
        // If the client doesn't provide Name, validation fails.
        //
        // Example:
        //
        // {
        //     "name": ""
        // }
        //
        // can result in a validation error.
        [Required]


        // [StringLength(100)] means the string can contain
        // a maximum of 100 characters.
        //
        // This protects the API from receiving unnecessarily
        // large values for this field.
        [StringLength(100)]


        // Name contains the employee's name.
        //
        // = string.Empty
        //
        // initializes the property with an empty string
        // instead of null.
        //
        // This helps satisfy nullable reference type
        // requirements and avoids an uninitialized property.
        public string Name { get; set; } = string.Empty;


        // ====================================================
        // EMPLOYEE EMAIL
        // ====================================================

        // Email is required.
        //
        // The request should contain an email value.
        [Required]


        // [EmailAddress] validates that the supplied value
        // follows an email-address format.
        //
        // Example of a valid-looking value:
        //
        // ashish@gmail.com
        //
        // Example of an invalid value:
        //
        // ashish
        //
        [EmailAddress]


        // Employee email address.
        public string Email { get; set; } = string.Empty;


        // ====================================================
        // DEPARTMENT
        // ====================================================

        // Department is required.
        //
        // The client must provide a department.
        [Required]


        // Maximum length is 100 characters.
        [StringLength(100)]


        // Example values:
        //
        // IT
        // HR
        // Finance
        // Administration
        // Operations
        //
        public string Department { get; set; } = string.Empty;


        // ====================================================
        // SALARY
        // ====================================================

        // [Range] defines the allowed numeric range.
        //
        // Minimum:
        // 1
        //
        // Maximum:
        // 100000000
        //
        // Therefore values outside this range will fail
        // validation.
        //
        // Example:
        //
        // Salary = 0
        //     ↓
        // Invalid
        //
        // Salary = 50000
        //     ↓
        // Valid
        //
        // Salary = 100000001
        //     ↓
        // Invalid
        [Range(1, 100000000)]


        // decimal is used for salary because salary is a
        // financial/monetary value.
        //
        // Decimal is generally preferred over float/double
        // for monetary values because it provides decimal
        // precision.
        //
        // Example:
        //
        // 75000.50
        //
        public decimal Salary { get; set; }
    }
}

