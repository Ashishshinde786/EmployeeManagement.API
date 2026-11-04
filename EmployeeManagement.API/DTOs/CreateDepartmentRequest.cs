using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    public class CreateDepartmentRequest
    {
        // Department name is mandatory.
        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Department name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Description is optional.
        [StringLength(500,
            ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}