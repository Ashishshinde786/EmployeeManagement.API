using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    /// <summary>
    /// Represents the data required to update an existing employee.
    /// The client sends this data to the update employee endpoint.
    /// </summary>
    public class UpdateEmployeeRequest
    {
        // Employee's full name.
        // The value is required and must be between 2 and 100 characters.
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // Employee's email address.
        // Required ensures that a value must be provided.
        // EmailAddress checks whether the value has a valid email format.
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Department in which the employee works.
        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        // Employee's salary.
        // Range prevents values outside the configured limits.
        [Range(0.01, 100000000)]
        public decimal Salary { get; set; }

        // Employee's contact number.
        // The exact validation depends on the attributes defined in the class.
        public string? PhoneNumber { get; set; }

        // Date on which the employee joined the organization.
        public DateTime JoiningDate { get; set; }

        // Employee's job title.
        public string? JobTitle { get; set; }
    }
}