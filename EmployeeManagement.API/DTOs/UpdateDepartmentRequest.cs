using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    public class UpdateDepartmentRequest
    {
        // Updated department name.
        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Department name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Updated description.
        [StringLength(500,
            ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        // Department active/inactive status.
        public bool IsActive { get; set; }
    }
}