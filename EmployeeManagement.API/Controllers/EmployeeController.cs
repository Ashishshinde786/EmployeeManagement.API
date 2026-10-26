// Provides request DTOs and response DTOs used by employee endpoints.
using EmployeeManagement.API.DTOs;

// Provides IEmployeeService, which contains the employee operations
// that this controller delegates to.
using EmployeeManagement.API.Services;

// Provides [Authorize] and role-based authorization attributes.
using Microsoft.AspNetCore.Authorization;

// Provides ControllerBase, routing attributes, and HTTP response methods.
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    // Marks this class as an API controller.
    // ASP.NET Core automatically validates DTOs that use validation
    // attributes such as [Required], [EmailAddress], and [Range].
    [ApiController]

    // Sets the base URL for this controller.
    // Unlike AuthController's dynamic route, this route is explicitly
    // defined as /api/employees.
    [Route("api/employees")]

    // Indicates that the controller produces JSON responses.
    [Produces("application/json")]

    // Requires the caller to be authenticated before accessing
    // endpoints in this controller, unless an endpoint explicitly
    // overrides this policy with an attribute such as [AllowAnonymous].
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        // Holds the employee service dependency.
        // The controller delegates business operations to this service
        // rather than directly accessing the database.
        private readonly IEmployeeService _employeeService;

        // ASP.NET Core dependency injection supplies the service.
        // Program.cs registers EmployeeService as the implementation
        // of IEmployeeService.
        public EmployeeController(IEmployeeService employeeService)
        {
            // Store the injected service for use by the action methods.
            _employeeService = employeeService;
        }

        // GET /api/employees?page=1&pageSize=10&department=IT
        //     &sortBy=name&sortOrder=asc
        //
        // Retrieves a page of employees, optionally filtered by department
        // and sorted by an allowed field.
        //
        // Any authenticated user can access this endpoint.
        [HttpGet]

        // Documents the expected response types for Swagger and API clients.
        [ProducesResponseType(
            typeof(PagedEmployeeResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllEmployees(
            // Page number requested by the client.
            // Defaults to page 1 when not supplied.
            int page = 1,

            // Maximum number of employees requested for one page.
            // Defaults to 10.
            int pageSize = 10,

            // Optional department filter.
            // Example: ?department=IT
            string? department = null,

            // Optional sorting field.
            // The repository supports name, salary, and department.
            string? sortBy = null,

            // Sort direction: "asc" or "desc".
            // Defaults to ascending order.
            string? sortOrder = "asc")
        {
            // Validate the page number.
            // Page numbers start at 1, not 0.
            if (page < 1)
            {
                // Return HTTP 400 when the requested page is invalid.
                return BadRequest("Page must be greater than 0.");
            }

            // Prevent invalid or excessively large page sizes.
            // This endpoint allows between 1 and 100 employees per page.
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(
                    "PageSize must be between 1 and 100.");
            }

            // Delegate filtering, sorting, pagination, and retrieval
            // to the employee service.
            //
            // The service calls the repository, maps database entities
            // to response DTOs, and calculates the total number of pages.
            var result = await _employeeService.GetPagedAsync(
                page,
                pageSize,
                department,
                sortBy,
                sortOrder);

            // Return HTTP 200 OK with the paginated employee response.
            // The response includes the employee list, current page,
            // page size, total records, and total pages.
            return Ok(result);
        }

        // GET /api/employees/10
        //
        // Retrieves one employee using the employee ID in the URL.
        // Any authenticated user can access this endpoint.
        [HttpGet("{id:int}")]

        // Documents the possible response types.
        [ProducesResponseType(
            typeof(EmployeeResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            // Ask the service to retrieve the employee with this ID.
            // The service returns an EmployeeResponse or null if missing.
            var employee = await _employeeService.GetByIdAsync(id);

            // Check whether the service found the requested employee.
            if (employee == null)
            {
                // Return HTTP 404 Not Found if the employee does not exist.
                return NotFound();
            }

            // Return HTTP 200 OK with the employee's response DTO.
            return Ok(employee);
        }

        // POST /api/employees
        //
        // Creates a new employee.
        // Only users with the Admin role can access this endpoint.
        [HttpPost]
        [Authorize(Roles = "Admin")]

        // A successful creation returns HTTP 201 Created.
        [ProducesResponseType(
            typeof(EmployeeResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateEmployee(
            // Contains the employee data supplied in the request body.
            // Its validation attributes are checked automatically
            // by ASP.NET Core when model validation runs.
            CreateEmployeeRequest request)
        {
            // Ask the service to create the employee.
            // The service maps the DTO to an Employee entity,
            // saves it through the repository, and returns a response DTO.
            var createdEmployee =
                await _employeeService.CreateAsync(request);

            // Return HTTP 201 Created.
            //
            // nameof(GetEmployeeById) identifies the action that can
            // retrieve the new employee.
            //
            // new { id = createdEmployee.Id } supplies the route value
            // for that action.
            //
            // createdEmployee becomes the JSON response body.
            // ASP.NET Core also generates a Location header pointing
            // to the newly created resource.
            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = createdEmployee.Id },
                createdEmployee);
        }

        // PUT /api/employees/10
        //
        // Updates an existing employee.
        // Only users with the Admin role can access this endpoint.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]

        // HTTP 204 means the update succeeded and no response body
        // is required.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEmployee(
            // Employee ID comes from the URL.
            int id,

            // Updated employee data comes from the request body.
            UpdateEmployeeRequest request)
        {
            // Ask the service to update the employee.
            // The service returns true if the update succeeds,
            // or false if the employee does not exist.
            var updated =
                await _employeeService.UpdateAsync(id, request);

            // If the service could not find the employee,
            // return HTTP 404 Not Found.
            if (!updated)
            {
                return NotFound();
            }

            // Return HTTP 204 No Content after a successful update.
            return NoContent();
        }

        // DELETE /api/employees/10
        //
        // Deletes an employee using the ID in the URL.
        // Only users with the Admin role can access this endpoint.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]

        // Documents the possible HTTP responses.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            // Ask the service to delete the employee.
            // The service first checks whether the employee exists.
            var deleted = await _employeeService.DeleteAsync(id);

            // If the employee does not exist, the service returns false.
            if (!deleted)
            {
                return NotFound();
            }

            // Return HTTP 204 No Content when deletion succeeds.
            return NoContent();
        }
    }
}