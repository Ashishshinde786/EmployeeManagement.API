using EmployeeManagement.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmployeeManagement.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ConflictException ex)
            {
                // ConflictException represents a business conflict,
                // such as a username that already exists.
                _logger.LogWarning(
                    ex,
                    "A business conflict occurred while processing the request.");

                await HandleConflictExceptionAsync(
                    context,
                    ex);
            }
            catch (Exception ex)
            {
                // Any unexpected exception is treated as
                // an internal server error.
                _logger.LogError(
                    ex,
                    "An unexpected error occurred while processing the request.");

                await HandleExceptionAsync(context);
            }
        }

        private static async Task HandleConflictExceptionAsync(
            HttpContext context,
            ConflictException ex)
        {
            context.Response.StatusCode =
                StatusCodes.Status409Conflict;

            context.Response.ContentType =
                "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = ex.Message
            };

            var json = JsonSerializer.Serialize(problemDetails);

            await context.Response.WriteAsync(json);
        }

        private static async Task HandleExceptionAsync(
            HttpContext context)
        {
            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.ContentType =
                "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred."
            };

            var json = JsonSerializer.Serialize(problemDetails);

            await context.Response.WriteAsync(json);
        }
    }
}