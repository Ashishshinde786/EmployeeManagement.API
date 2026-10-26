// Imports validation attributes such as [Required], [EmailAddress],
// [Range], [Phone], and [StringLength].
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    /// <summary>
    /// Defines the data required to create a new employee.
    ///
    /// This DTO receives data from the HTTP request body.
    /// ASP.NET Core uses its validation attributes to check the
    /// incoming data before the controller processes the request.
    ///
    /// It does not save data to the database.
    /// EmployeeService and EmployeeRepository handle that work.
    /// </summary>
    public class CreateEmployeeRequest
    {
        // Employee's full name.
        //
        // [Required] means the value must be provided.
        // [StringLength] restricts the name to 2-100 characters.
        //
        // Example: "Rahul Patil"
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // Employee's email address.
        //
        // [Required] requires a value.
        // [EmailAddress] checks whether the value has a valid
        // email-like format.
        // [StringLength(256)] sets the maximum length to 256 characters.
        //
        // Example: "rahul@example.com"
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        // Department in which the employee works.
        //
        // [Required] requires a value.
        // [StringLength(100)] limits the department name to 100 characters.
        //
        // Example: "IT", "HR", or "Finance"
        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        // Employee's salary.
        //
        // [Range] restricts the numeric value to the configured range.
        // The minimum is 0 and the maximum is 100,000,000.
        //
        // decimal is suitable for financial values because it represents
        // decimal numbers more appropriately than floating-point types.
        //
        // Example: 65000.00
        [Range(0, 100000000)]
        public decimal Salary { get; set; }

        // Employee's phone number.
        //
        // The question mark (?) makes this property nullable.
        // This means a phone number can be omitted or be null.
        //
        // [Phone] checks whether the value resembles a phone number.
        // [StringLength(20)] limits its length to 20 characters.
        //
        // A string is used because phone numbers can contain
        // leading zeros, country codes, spaces, or a plus sign.
        //
        // Example: "+91 9876543210"
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        // Employee's joining date.
        //
        // DateTime? means the property can contain a date/time value
        // or null.
        //
        // [Required] makes the joining date mandatory during request
        // validation, even though the C# property is nullable.
        //
        // The nullable type allows validation to detect a missing value
        // instead of silently using DateTime's default date.
        //
        // Example: "2026-10-26"
        [Required]
        public DateTime? JoiningDate { get; set; }

        // Employee's job title.
        //
        // This property is optional because it has no [Required] attribute
        // and its type is nullable.
        //
        // [StringLength(100)] limits the value to 100 characters.
        //
        // Example: "Software Developer"
        [StringLength(100)]
        public string? JobTitle { get; set; }
    }
}