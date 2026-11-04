
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class Department
    {
        // Unique ID of the department.
        public int Id { get; set; }

        // Department name must be provided.
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Optional description of the department.
        [StringLength(500)]
        public string? Description { get; set; }

        // Indicates whether the department is active.
        // We will use this for soft deletion.
        public bool IsActive { get; set; } = true;

        // Date and time when the department was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Date and time when the department was last updated.
        public DateTime? UpdatedAt { get; set; }
    }
}
