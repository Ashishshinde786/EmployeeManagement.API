using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Produces("application/json")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // ============================================================
        // GET ALL EMPLOYEES
        // User + Admin can access
        // Pagination + Filtering + Sorting
        // ============================================================

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllEmployees(
            int page = 1,
            int pageSize = 10,
            string? department = null,
            string? sortBy = null,
            string? sortOrder = "asc")
        {
            // Validate page number.
            if (page < 1)
            {
                return BadRequest(
                    "Page must be greater than 0.");
            }

            // Validate page size.
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(
                    "PageSize must be between 1 and 100.");
            }

            // Get employees using pagination,
            // filtering and sorting.
            var result =
                await _employeeService.GetPagedAsync(
                    page,
                    pageSize,
                    department,
                    sortBy,
                    sortOrder);

            return Ok(result);
        }


        // ============================================================
        // GET EMPLOYEE BY ID
        // User + Admin can access
        // ============================================================

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            // Find employee by ID.
            var employee =
                await _employeeService.GetByIdAsync(id);

            // Employee does not exist.
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }


        // ============================================================
        // CREATE EMPLOYEE
        // Only Admin can access
        // ============================================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateEmployee(
            CreateEmployeeRequest request)
        {
            // Create a new employee.
            var createdEmployee =
                await _employeeService.CreateAsync(request);

            // Return HTTP 201 Created.
            //
            // CreatedAtAction also provides the URL
            // for retrieving the newly created employee.
            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = createdEmployee.Id },
                createdEmployee);
        }


        // ============================================================
        // UPDATE EMPLOYEE
        // Only Admin can access
        // ============================================================

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            UpdateEmployeeRequest request)
        {
            // Update the employee.
            var updated =
                await _employeeService.UpdateAsync(
                    id,
                    request);

            // Employee was not found.
            if (!updated)
            {
                return NotFound();
            }

            // Update successful.
            return NoContent();
        }


        // ============================================================
        // DELETE EMPLOYEE
        // Only Admin can access
        // ============================================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            // Delete the employee.
            var deleted =
                await _employeeService.DeleteAsync(id);

            // Employee was not found.
            if (!deleted)
            {
                return NotFound();
            }

            // Delete successful.
            return NoContent();
        }
    }
}