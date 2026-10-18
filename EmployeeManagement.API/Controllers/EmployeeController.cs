
// ============================================================
// NAMESPACES / IMPORTS
// ============================================================

// Contains our DTO classes.
//
// DTO = Data Transfer Object.
//
// DTOs define what data the API accepts from the client
// and what data we want to transfer between layers.
//
// Examples:
//
// CreateEmployeeRequest
// UpdateEmployeeRequest
//
// Using DTOs prevents us from directly exposing our
// database entity models through the API.
using EmployeeManagement.API.DTOs;


// Contains employee service interfaces and implementations.
//
// The controller communicates with the Service layer
// through IEmployeeService.
using EmployeeManagement.API.Services;


// Provides authentication and authorization attributes.
//
// We use:
//
// [Authorize]
// [Authorize(Roles = "Admin")]
//
// to protect our API endpoints.
using Microsoft.AspNetCore.Authorization;


// Provides ASP.NET Core MVC functionality.
//
// Contains:
//
// ControllerBase
// ApiController
// HttpGet
// HttpPost
// HttpPut
// HttpDelete
// IActionResult
// ProducesResponseType
//
// etc.
using Microsoft.AspNetCore.Mvc;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.Controllers
{
    // ========================================================
    // API CONTROLLER CONFIGURATION
    // ========================================================

    // [ApiController] tells ASP.NET Core that this class
    // is an API controller.
    //
    // It provides useful API behavior such as:
    //
    // 1. Automatic model binding
    // 2. Automatic validation
    // 3. Automatic 400 Bad Request for invalid models
    // 4. Better API parameter handling
    //
    [ApiController]


    // ========================================================
    // ROUTING
    // ========================================================

    // Defines the base route for all endpoints
    // inside this controller.
    //
    // Because the route is:
    //
    // api/employees
    //
    // our endpoints become:
    //
    // GET     /api/employees
    // GET     /api/employees/1
    // POST    /api/employees
    // PUT     /api/employees/1
    // DELETE  /api/employees/1
    //
    [Route("api/employees")]


    // ========================================================
    // RESPONSE FORMAT
    // ========================================================

    // Tells ASP.NET Core that this controller produces
    // JSON responses.
    //
    // JSON is commonly used for communication between
    // frontend applications and REST APIs.
    //
    // Example:
    //
    // {
    //     "id": 1,
    //     "name": "Ashish"
    // }
    //
    [Produces("application/json")]


    // ========================================================
    // AUTHENTICATION
    // ========================================================

    // Protect the entire controller.
    //
    // This means every endpoint in this controller requires
    // an authenticated user unless authorization is explicitly
    // changed for a particular action.
    //
    // The client must send:
    //
    // Authorization: Bearer <JWT>
    //
    // Authentication checks:
    //
    // "Is this a valid authenticated user?"
    //
    [Authorize]


    // ========================================================
    // EMPLOYEE CONTROLLER
    // ========================================================

    public class EmployeeController : ControllerBase
    {
        // ====================================================
        // EMPLOYEE SERVICE
        // ====================================================

        // The controller should not directly communicate
        // with Entity Framework or SQL Server.
        //
        // Instead, it communicates with the service layer.
        //
        // Controller
        //     ↓
        // IEmployeeService
        //     ↓
        // EmployeeService
        //
        private readonly IEmployeeService _employeeService;


        // ====================================================
        // CONSTRUCTOR / DEPENDENCY INJECTION
        // ====================================================

        // ASP.NET Core Dependency Injection automatically
        // provides an implementation of IEmployeeService.
        //
        // Program.cs contains:
        //
        // builder.Services.AddScoped<
        //     IEmployeeService,
        //     EmployeeService>();
        //
        // Therefore ASP.NET Core creates EmployeeService
        // and injects it into this controller.
        public EmployeeController(IEmployeeService employeeService)
        {
            // Store the injected service.
            _employeeService = employeeService;
        }


        // ============================================================
        // GET ALL EMPLOYEES
        // ============================================================
        //
        // Accessible by:
        //
        // Any authenticated user.
        //
        // Because the controller has:
        //
        // [Authorize]
        //
        // but this action does not have:
        //
        // [Authorize(Roles = "Admin")]
        //
        // both User and Admin can access it.
        //
        // This endpoint supports:
        //
        // Pagination
        // Filtering
        // Sorting
        // ============================================================

        // HTTP GET request.
        //
        // Endpoint:
        //
        // GET /api/employees
        //
        [HttpGet]


        // Swagger/OpenAPI documentation.
        //
        // Successful response:
        //
        // 200 OK
        //
        [ProducesResponseType(StatusCodes.Status200OK)]


        // Invalid pagination parameters:
        //
        // 400 Bad Request
        //
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        // User is not authenticated:
        //
        // 401 Unauthorized
        //
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        // ====================================================
        // GET ALL EMPLOYEES METHOD
        // ====================================================

        public async Task<IActionResult> GetAllEmployees(

            // Page number.
            //
            // Default:
            // page = 1
            //
            // Example:
            //
            // ?page=2
            int page = 1,


            // Number of records per page.
            //
            // Default:
            // pageSize = 10
            //
            // Example:
            //
            // ?pageSize=20
            int pageSize = 10,


            // Optional department filter.
            //
            // Example:
            //
            // ?department=IT
            //
            // ?department=HR
            //
            // The ? means this value can be null.
            string? department = null,


            // Optional field used for sorting.
            //
            // Example:
            //
            // ?sortBy=name
            //
            // ?sortBy=salary
            string? sortBy = null,


            // Sorting direction.
            //
            // Default is ascending.
            //
            // Example:
            //
            // ?sortOrder=asc
            //
            // ?sortOrder=desc
            string? sortOrder = "asc")
        {
            // =================================================
            // VALIDATE PAGE
            // =================================================

            // Page numbers should start from 1.
            //
            // Page 0 or negative pages don't make sense.
            //
            // Example:
            //
            // /api/employees?page=0
            //
            // returns:
            //
            // 400 Bad Request
            if (page < 1)
            {
                return BadRequest(
                    "Page must be greater than 0.");
            }


            // =================================================
            // VALIDATE PAGE SIZE
            // =================================================

            // We don't allow unlimited records to be returned.
            //
            // Example:
            //
            // pageSize = 100000
            //
            // could cause a large database query and
            // unnecessary memory/network usage.
            //
            // Therefore we allow:
            //
            // 1 to 100 records per page.
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(
                    "PageSize must be between 1 and 100.");
            }


            // =================================================
            // CALL SERVICE
            // =================================================

            // Pass pagination, filtering and sorting
            // information to the service layer.
            //
            // The controller is responsible for handling
            // HTTP concerns.
            //
            // The service is responsible for business logic
            // and data retrieval logic.
            var result =
                await _employeeService.GetPagedAsync(
                    page,
                    pageSize,
                    department,
                    sortBy,
                    sortOrder);


            // =================================================
            // RETURN RESPONSE
            // =================================================

            // Return HTTP 200 OK with the result.
            //
            // ASP.NET Core automatically serializes the result
            // into JSON.
            return Ok(result);
        }


        // ============================================================
        // GET EMPLOYEE BY ID
        // ============================================================
        //
        // Accessible by:
        //
        // User + Admin
        //
        // because the controller already has [Authorize].
        //
        // ============================================================

        // HTTP GET request with route parameter.
        //
        // Endpoint:
        //
        // GET /api/employees/{id}
        //
        // Example:
        //
        // GET /api/employees/10
        //
        [HttpGet("{id}")]


        // Possible response:
        //
        // 200 OK
        //
        [ProducesResponseType(StatusCodes.Status200OK)]


        // User is not authenticated:
        //
        // 401 Unauthorized
        //
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        // Employee doesn't exist:
        //
        // 404 Not Found
        //
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        // ====================================================
        // GET EMPLOYEE BY ID
        // ====================================================

        public async Task<IActionResult> GetEmployeeById(int id)
        {
            // =================================================
            // FIND EMPLOYEE
            // =================================================

            // Send the employee ID to the service layer.
            //
            // The service will retrieve the employee
            // from the database.
            var employee =
                await _employeeService.GetByIdAsync(id);


            // =================================================
            // EMPLOYEE NOT FOUND
            // =================================================

            // If the service returns null, no employee
            // exists with this ID.
            //
            // HTTP 404 means:
            //
            // "The requested resource was not found."
            if (employee == null)
            {
                return NotFound();
            }


            // =================================================
            // SUCCESS
            // =================================================

            // Return HTTP 200 OK and employee data.
            return Ok(employee);
        }


        // ============================================================
        // CREATE EMPLOYEE
        // ============================================================
        //
        // Only Admin users can create employees.
        //
        // ============================================================

        // This overrides the controller-level [Authorize]
        // with a more specific authorization requirement.
        //
        // The user must:
        //
        // 1. Be authenticated.
        // 2. Have the "Admin" role.
        //
        [Authorize(Roles = "Admin")]


        // HTTP POST:
        //
        // POST /api/employees
        //
        [HttpPost]


        // Successful creation:
        //
        // 201 Created
        //
        [ProducesResponseType(StatusCodes.Status201Created)]


        // Invalid request:
        //
        // 400 Bad Request
        //
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        // No valid authentication:
        //
        // 401 Unauthorized
        //
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        // Authenticated but not an Admin:
        //
        // 403 Forbidden
        //
        [ProducesResponseType(StatusCodes.Status403Forbidden)]


        // ====================================================
        // CREATE EMPLOYEE
        // ====================================================

        public async Task<IActionResult> CreateEmployee(
            CreateEmployeeRequest request)
        {
            // =================================================
            // CREATE EMPLOYEE
            // =================================================

            // Send the request DTO to the service.
            //
            // Service layer handles the actual business logic.
            var createdEmployee =
                await _employeeService.CreateAsync(request);


            // =================================================
            // RETURN 201 CREATED
            // =================================================

            // CreatedAtAction returns:
            //
            // 201 Created
            //
            // and also provides the URL for retrieving
            // the newly created employee.
            //
            // Example:
            //
            // New employee ID = 10
            //
            // Location:
            //
            // /api/employees/10
            //
            // nameof(GetEmployeeById)
            // tells ASP.NET Core which action can retrieve
            // the newly created resource.
            return CreatedAtAction(
                nameof(GetEmployeeById),

                // Route values required by GetEmployeeById.
                new { id = createdEmployee.Id },

                // Body of the response.
                createdEmployee);
        }


        // ============================================================
        // UPDATE EMPLOYEE
        // ============================================================
        //
        // Only Admin users can update employees.
        //
        // ============================================================

        [Authorize(Roles = "Admin")]


        // HTTP PUT:
        //
        // PUT /api/employees/{id}
        //
        // Example:
        //
        // PUT /api/employees/10
        //
        [HttpPut("{id}")]


        // Successful update:
        //
        // 204 No Content
        //
        [ProducesResponseType(StatusCodes.Status204NoContent)]


        // Invalid request:
        //
        // 400 Bad Request
        //
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        // User not authenticated:
        //
        // 401 Unauthorized
        //
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        // User authenticated but not Admin:
        //
        // 403 Forbidden
        //
        [ProducesResponseType(StatusCodes.Status403Forbidden)]


        // Employee not found:
        //
        // 404 Not Found
        //
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        // ====================================================
        // UPDATE EMPLOYEE
        // ====================================================

        public async Task<IActionResult> UpdateEmployee(
            int id,
            UpdateEmployeeRequest request)
        {
            // =================================================
            // UPDATE EMPLOYEE
            // =================================================

            // Send employee ID and update DTO to service.
            //
            // The service returns true if the employee
            // was successfully updated.
            var updated =
                await _employeeService.UpdateAsync(
                    id,
                    request);


            // =================================================
            // EMPLOYEE NOT FOUND
            // =================================================

            // If false is returned, the employee doesn't exist.
            if (!updated)
            {
                return NotFound();
            }


            // =================================================
            // UPDATE SUCCESSFUL
            // =================================================

            // 204 No Content means:
            //
            // "The request was successfully processed,
            // but there is no response body to return."
            return NoContent();
        }


        // ============================================================
        // DELETE EMPLOYEE
        // ============================================================
        //
        // Only Admin users can delete employees.
        //
        // ============================================================

        [Authorize(Roles = "Admin")]


        // HTTP DELETE:
        //
        // DELETE /api/employees/{id}
        //
        // Example:
        //
        // DELETE /api/employees/10
        //
        [HttpDelete("{id}")]


        // Successful deletion:
        //
        // 204 No Content
        //
        [ProducesResponseType(StatusCodes.Status204NoContent)]


        // User not authenticated:
        //
        // 401 Unauthorized
        //
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        // User authenticated but not Admin:
        //
        // 403 Forbidden
        //
        [ProducesResponseType(StatusCodes.Status403Forbidden)]


        // Employee doesn't exist:
        //
        // 404 Not Found
        //
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        // ====================================================
        // DELETE EMPLOYEE
        // ====================================================

        public async Task<IActionResult> DeleteEmployee(int id)
        {
            // =================================================
            // DELETE EMPLOYEE
            // =================================================

            // Pass employee ID to the service layer.
            //
            // Service performs the actual delete operation.
            var deleted =
                await _employeeService.DeleteAsync(id);


            // =================================================
            // EMPLOYEE NOT FOUND
            // =================================================

            // If false is returned, there was no employee
            // with the specified ID.
            if (!deleted)
            {
                return NotFound();
            }


            // =================================================
            // DELETE SUCCESSFUL
            // =================================================

            // Return HTTP 204 No Content.
            return NoContent();
        }
    }
}

