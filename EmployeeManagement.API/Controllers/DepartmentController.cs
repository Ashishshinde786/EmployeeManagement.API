
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // =====================================================
        // 1. GET: api/Department
        // Get all active departments.
        // Requires authentication.
        // =====================================================

        [HttpGet]
        public async Task<ActionResult<List<DepartmentResponse>>> GetAll()
        {
            var departments = await _departmentService.GetAllAsync();

            return Ok(departments);
        }

        // =====================================================
        // 2. GET: api/Department/{id}
        // Get department by ID.
        // Requires authentication.
        // =====================================================

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentResponse>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Department ID must be greater than zero."
                });
            }

            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound(new
                {
                    message = $"Department with ID {id} was not found."
                });
            }

            return Ok(department);
        }

        // =====================================================
        // 3. POST: api/Department
        // Create a new department.
        // Admin role required.
        // =====================================================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentResponse>> Create(
            [FromBody] CreateDepartmentRequest request)
        {
            var department = await _departmentService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = department.Id },
                department);
        }

        // =====================================================
        // 4. PUT: api/Department/{id}
        // Update an existing department.
        // Admin role required.
        // =====================================================

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentResponse>> Update(
            int id,
            [FromBody] UpdateDepartmentRequest request)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Department ID must be greater than zero."
                });
            }

            var department = await _departmentService.UpdateAsync(id, request);

            if (department == null)
            {
                return NotFound(new
                {
                    message = $"Department with ID {id} was not found."
                });
            }

            return Ok(department);
        }

        // =====================================================
        // 5. DELETE: api/Department/{id}
        // Soft delete a department.
        // Admin role required.
        // =====================================================

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Department ID must be greater than zero."
                });
            }

            var deleted = await _departmentService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Department with ID {id} was not found."
                });
            }

            return NoContent();
        }
    }
}
