using System.Collections.Generic;

namespace EmployeeManagement.API.DTOs
{
    /// <summary>
    /// Represents the API response for a paginated list of employees.
    /// It contains employee data and pagination information.
    /// </summary>
    public class PagedEmployeeResponse
    {
        /// <summary>
        /// Contains the employee records returned for the requested page.
        ///
        /// EmployeeResponse is used instead of the Employee entity
        /// so the API returns the response DTO structure.
        ///
        /// new() creates an empty list by default, avoiding a null list
        /// when an instance of this class is created.
        /// </summary>
        public List<EmployeeResponse> Employees { get; set; } = new();

        /// <summary>
        /// The current page number requested by the client.
        /// For example, Page = 2 means the client requested page 2.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The maximum number of employee records requested per page.
        /// For example, PageSize = 10 means up to 10 records per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of employee records matching the query,
        /// before pagination is applied.
        /// For example, if 100 employees match the filter,
        /// TotalRecords will be 100.
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// The total number of pages required to display all matching
        /// employee records using the requested page size.
        ///
        /// For example, 100 records with 10 records per page
        /// require 10 pages.
        /// </summary>
        public int TotalPages { get; set; }
    }
}