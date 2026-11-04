namespace EmployeeManagement.API.DTOs
{
    public class DepartmentResponse
    {
        // Unique department ID.
        public int Id { get; set; }

        // Department name.
        public string Name { get; set; } = string.Empty;

        // Optional department description.
        public string? Description { get; set; }

        // Indicates whether the department is active.
        public bool IsActive { get; set; }

        // Department creation date.
        public DateTime CreatedAt { get; set; }

        // Department last update date.
        public DateTime? UpdatedAt { get; set; }
    }
}