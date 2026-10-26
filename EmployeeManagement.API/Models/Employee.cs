using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    /// <summary>
    /// Represents an employee stored in the database.
    /// Entity Framework Core uses this class to map employee data
    /// between C# objects and database records.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Unique identifier for each employee.
        /// By EF Core conventions, an integer property named Id
        /// is normally configured as the primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Employee's full name.
        /// Required means a value must be provided during validation.
        /// StringLength limits the name to 100 characters.
        /// string.Empty initializes the property with an empty string.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Employee's email address.
        /// Required specifies that the value is mandatory.
        /// EmailAddress validates the email's format.
        /// StringLength limits the value to 256 characters.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Department in which the employee works.
        /// Required specifies that a value must be provided.
        /// StringLength limits the value to 100 characters.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Employee's salary.
        /// decimal is suitable for financial values because it supports
        /// decimal precision more appropriately than floating-point types.
        /// Range specifies that the salary must be zero or greater.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        /// <summary>
        /// Employee's contact number.
        /// The question mark (?) makes this property nullable,
        /// meaning the value can be null when it is not available.
        /// Phone validates the value's phone-number format.
        /// StringLength limits the value to 20 characters.
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Date on which the employee joined the organization.
        /// DateTime? allows the value to be null when the date is unknown.
        /// </summary>
        public DateTime? JoiningDate { get; set; }

        /// <summary>
        /// Employee's job title, such as Software Developer.
        /// This property is optional because it is nullable.
        /// StringLength limits the value to 100 characters.
        /// </summary>
        [StringLength(100)]
        public string? JobTitle { get; set; }
    }
}