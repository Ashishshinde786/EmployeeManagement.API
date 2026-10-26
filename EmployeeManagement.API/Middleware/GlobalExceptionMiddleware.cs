
// Imports the custom exception used when a business conflict occurs,
// such as registering a username that already exists.
using EmployeeManagement.API.Exceptions;

// Provides ProblemDetails, a standard format for API error responses.
using Microsoft.AspNetCore.Mvc;

// Provides JSON serialization functionality.
using System.Text.Json;

namespace EmployeeManagement.API.Middleware
{
    // GlobalExceptionMiddleware catches exceptions thrown while
    // processing HTTP requests and converts them into HTTP error responses.
    public class GlobalExceptionMiddleware
    {
        // Holds the next middleware component in the HTTP request pipeline.
        // It passes the request to the next component.
        private readonly RequestDelegate _next;

        // Used to record warning and error information in application logs.
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        // Constructor: ASP.NET Core injects the next middleware and logger.
        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            // Store the next middleware so this middleware can call it.
            _next = next;

            // Store the logger so exceptions can be recorded.
            _logger = logger;
        }

        // This method runs for each HTTP request that reaches this middleware.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass the request to the next middleware or endpoint.
                // If an exception occurs downstream, execution moves to a catch block.
                await _next(context);
            }
            catch (ConflictException ex)
            {
                // A known business conflict occurred.
                // Log it as a warning because it is an expected application-level issue.
                _logger.LogWarning(
                    ex,
                    "A business conflict occurred while processing the request.");

                // Return HTTP 409 Conflict with a standard error response.
                // The exception message is safe to expose for this known exception
                // only if the application controls the message content.
                await WriteProblemAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    ex.Message);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions that were not caught elsewhere.
                // Log the exception, including its details, for developers to investigate.
                _logger.LogError(
                    ex,
                    "An unexpected error occurred while processing the request.");

                // Return HTTP 500 Internal Server Error.
                // Do not expose internal exception details to the client because
                // they may contain database details, file paths, or other sensitive data.
                await WriteProblemAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred.");
            }
        }

        // Creates and sends a standardized ProblemDetails JSON response.
        // This method is static because it does not use instance fields.
        private static async Task WriteProblemAsync(
            HttpContext context,
            int statusCode,
            string title,
            string detail)
        {
            // If the response has already started, its status and headers
            // may already have been sent to the client and cannot be replaced.
            if (context.Response.HasStarted)
            {
                return;
            }

            // Clear any response headers and body content that have not yet
            // been sent, so we can create a clean error response.
            context.Response.Clear();

            // Set the HTTP status code, such as 409 or 500.
            context.Response.StatusCode = statusCode;

            // Tell the client that the response body follows the Problem Details
            // JSON media type.
            context.Response.ContentType = "application/problem+json";

            // Create a standard object containing information about the error.
            var problemDetails = new ProblemDetails
            {
                Status = statusCode, // Numeric HTTP status code.
                Title = title,       // Short description of the error.
                Detail = detail      // Explanation safe to show to the client.
            };

            // Convert the ProblemDetails object into JSON and write it
            // to the HTTP response body.
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(problemDetails));
        }
    }
}
