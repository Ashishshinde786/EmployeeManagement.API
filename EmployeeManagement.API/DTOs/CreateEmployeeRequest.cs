using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    public class CreateEmployeeRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Range(1, 100000000)]
        public decimal Salary { get; set; }
    }
}