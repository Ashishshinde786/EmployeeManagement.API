
// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.DTOs
{
    // ========================================================
    // PAGED EMPLOYEE RESPONSE DTO
    // ========================================================

    // PagedEmployeeResponse is an OUTPUT DTO.
    //
    // It is used when the API returns a paginated list of
    // employees.
    //
    // Instead of returning thousands of employees in one
    // response, we return a small number of employees at a time.
    //
    // Example:
    //
    // If the database contains 1000 employees and the client
    // requests:
    //
    // page = 1
    // pageSize = 10
    //
    // The API returns only employees 1-10.
    //
    // The response also contains information such as:
    //
    // - Current page
    // - Page size
    // - Total number of records
    // - Total number of pages
    //
    // This information helps the frontend build pagination
    // buttons such as:
    //
    // Previous | 1 | 2 | 3 | 4 | 5 | Next
    //
    public class PagedEmployeeResponse
    {
        // ====================================================
        // EMPLOYEE LIST
        // ====================================================

        // Contains the employees belonging to the current page.
        //
        // EmployeeResponse is used instead of the Employee
        // database entity because EmployeeResponse is our
        // API response DTO.
        //
        // Example:
        //
        // If pageSize = 2:
        //
        // Employees = [
        //     Employee 1,
        //     Employee 2
        // ]
        //
        // List<EmployeeResponse>
        // means this property can contain multiple
        // EmployeeResponse objects.
        //
        // = new()
        // initializes an empty list.
        //
        // This prevents Employees from being null when there
        // are no employees to return.
        public List<EmployeeResponse> Employees { get; set; } = new();


        // ====================================================
        // CURRENT PAGE
        // ====================================================

        // Stores the current page number requested by the client.
        //
        // Example:
        //
        // GET /api/employees?page=2&pageSize=10
        //
        // Page = 2
        //
        // This tells the frontend that the returned employees
        // belong to page 2.
        public int Page { get; set; }


        // ====================================================
        // PAGE SIZE
        // ====================================================

        // Stores how many employees are requested per page.
        //
        // Example:
        //
        // pageSize = 10
        //
        // means the API returns a maximum of 10 employees
        // for the current page.
        //
        // The actual number can be smaller on the last page.
        public int PageSize { get; set; }


        // ====================================================
        // TOTAL RECORDS
        // ====================================================

        // Stores the total number of employee records available
        // in the database after applying the relevant filters.
        //
        // Example:
        //
        // Suppose the database contains:
        //
        // 100 employees
        //
        // Then:
        //
        // TotalRecords = 100
        //
        // This value helps the frontend understand how many
        // records exist in total.
        public int TotalRecords { get; set; }


        // ====================================================
        // TOTAL PAGES
        // ====================================================

        // Stores the total number of pages available.
        //
        // For example:
        //
        // TotalRecords = 100
        // PageSize = 10
        //
        // TotalPages = 10
        //
        // Another example:
        //
        // TotalRecords = 95
        // PageSize = 10
        //
        // TotalPages = 10
        //
        // Because the last page will contain only 5 employees.
        //
        // Usually this value is calculated as:
        //
        // TotalPages = Ceiling(TotalRecords / PageSize)
        //
        // For example:
        //
        // 95 / 10 = 9.5
        //
        // Ceiling(9.5) = 10
        //
        // So we need 10 pages.
        public int TotalPages { get; set; }
    }
}
