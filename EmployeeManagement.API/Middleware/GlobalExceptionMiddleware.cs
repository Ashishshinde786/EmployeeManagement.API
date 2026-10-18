
// ============================================================
// NAMESPACES / IMPORTS
// ============================================================

// Contains our custom application exceptions.
//
// Example:
//
// ConflictException
//
// We use custom exceptions to represent specific business
// situations instead of returning errors directly from every
// controller.
using EmployeeManagement.API.Exceptions;


// Provides ASP.NET Core MVC classes.
//
// We use ProblemDetails from this namespace.
//
// ProblemDetails provides a standard structure for returning
// API errors to clients.
using Microsoft.AspNetCore.Mvc;


// Provides JSON serialization functionality.
//
// We use JsonSerializer to convert our ProblemDetails object
// into JSON before sending it to the client.
using System.Text.Json;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.Middleware
{
    // ========================================================
    // GLOBAL EXCEPTION MIDDLEWARE
    // ========================================================

    // Middleware is a component that sits inside the
    // ASP.NET Core HTTP request pipeline.
    //
    // Its main responsibility here is to catch exceptions
    // that occur while processing an HTTP request.
    //
    // Instead of putting try-catch blocks inside every
    // controller:
    //
    // Controller 1 → try/catch
    // Controller 2 → try/catch
    // Controller 3 → try/catch
    //
    // we handle exceptions centrally:
    //
    // Request
    //    ↓
    // GlobalExceptionMiddleware
    //    ↓
    // Controller
    //    ↓
    // Service
    //    ↓
    // Repository
    //    ↓
    // Exception
    //    ↓
    // GlobalExceptionMiddleware
    //    ↓
    // Standard error response
    //
    public class GlobalExceptionMiddleware
    {
        // ====================================================
        // REQUEST DELEGATE
        // ====================================================

        // RequestDelegate represents the NEXT middleware
        // component in the HTTP pipeline.
        //
        // In simple words:
        //
        // "_next tells this middleware where to send
        // the request next."
        //
        // Example pipeline:
        //
        // GlobalExceptionMiddleware
        //          ↓
        //      Authentication
        //          ↓
        //      Authorization
        //          ↓
        //       Controller
        //
        // _next() is used to continue that pipeline.
        private readonly RequestDelegate _next;


        // ====================================================
        // LOGGER
        // ====================================================

        // ILogger is ASP.NET Core's built-in logging
        // abstraction.
        //
        // We use it to record exceptions and important
        // application events.
        //
        // Logs can later be sent to:
        //
        // - Console
        // - File
        // - Application Insights
        // - Serilog
        // - ELK
        // - Cloud logging platforms
        //
        private readonly ILogger<GlobalExceptionMiddleware> _logger;


        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // ASP.NET Core Dependency Injection automatically
        // provides:
        //
        // RequestDelegate
        // ILogger<GlobalExceptionMiddleware>
        //
        // when the middleware is created.
        //
        // This is Dependency Injection.
        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            // Store the next middleware in the pipeline.
            _next = next;


            // Store the logger so we can write logs later.
            _logger = logger;
        }


        // ====================================================
        // INVOKE ASYNC
        // ====================================================

        // ASP.NET Core middleware normally exposes an
        // Invoke() or InvokeAsync() method.
        //
        // ASP.NET Core calls this method for every incoming
        // HTTP request that passes through this middleware.
        //
        // HttpContext contains information about the current
        // HTTP request and response.
        //
        // Examples:
        //
        // context.Request
        // context.Response
        // context.User
        // context.Request.Path
        //
        public async Task InvokeAsync(HttpContext context)
        {
            // =================================================
            // TRY BLOCK
            // =================================================

            // We place the next part of the request pipeline
            // inside try-catch.
            //
            // This allows us to catch exceptions generated
            // by controllers, services, repositories, etc.
            try
            {
                // Continue processing the HTTP request.
                //
                // _next(context) sends the request to the next
                // middleware component.
                //
                // Eventually the request reaches:
                //
                // Controller
                //      ↓
                // Service
                //      ↓
                // Repository
                //      ↓
                // Database
                //
                await _next(context);
            }


            // =================================================
            // HANDLE BUSINESS CONFLICT
            // =================================================

            // Catch our custom ConflictException.
            //
            // This exception represents a business conflict.
            //
            // Example:
            //
            // User tries to register:
            //
            // ashish@gmail.com
            //
            // But that email already exists.
            //
            // Service may throw:
            //
            // throw new ConflictException(
            //     "Email already exists.");
            //
            // Instead of returning 500 Internal Server Error,
            // we return:
            //
            // 409 Conflict
            catch (ConflictException ex)
            {
                // Log the business conflict as a Warning.
                //
                // Warning is appropriate because this may be
                // an expected business condition rather than
                // an application crash.
                //
                // "ex" is included so the exception details
                // are available in the logs.
                _logger.LogWarning(
                    ex,
                    "A business conflict occurred while processing the request.");


                // Convert the ConflictException into a proper
                // HTTP 409 response.
                await HandleConflictExceptionAsync(
                    context,
                    ex);
            }


            // =================================================
            // HANDLE ALL OTHER EXCEPTIONS
            // =================================================

            // This catches any unexpected exception that was
            // not specifically handled above.
            //
            // Examples:
            //
            // - Database failure
            // - NullReferenceException
            // - Invalid operation
            // - Unexpected application error
            // - External service failure
            //
            // These are treated as:
            //
            // 500 Internal Server Error
            catch (Exception ex)
            {
                // Log the unexpected exception as an Error.
                //
                // This is important for debugging production
                // problems.
                _logger.LogError(
                    ex,
                    "An unexpected error occurred while processing the request.");


                // Return a safe generic error response.
                //
                // We intentionally don't return ex.Message
                // to the client.
                //
                // Exception messages can contain sensitive
                // implementation details.
                await HandleExceptionAsync(context);
            }
        }


        // ====================================================
        // HANDLE CONFLICT EXCEPTION
        // ====================================================

        // This method creates the HTTP response for
        // ConflictException.
        //
        // HTTP status:
        //
        // 409 Conflict
        //
        // is appropriate when the request conflicts with the
        // current state of the resource.
        //
        // Example:
        //
        // POST /api/auth/register
        //
        // Email already exists.
        //
        // Response:
        //
        // 409 Conflict
        private static async Task HandleConflictExceptionAsync(
            HttpContext context,
            ConflictException ex)
        {
            // ------------------------------------------------
            // HTTP STATUS CODE
            // ------------------------------------------------

            // Set response status code to 409.
            //
            // 409 means:
            //
            // "The request could not be completed because
            // it conflicts with the current state of the
            // resource."
            context.Response.StatusCode =
                StatusCodes.Status409Conflict;


            // ------------------------------------------------
            // CONTENT TYPE
            // ------------------------------------------------

            // Tell the client that the response contains
            // Problem Details JSON.
            //
            // application/problem+json is a standard media
            // type for API error responses.
            context.Response.ContentType =
                "application/problem+json";


            // ------------------------------------------------
            // CREATE PROBLEM DETAILS
            // ------------------------------------------------

            // ProblemDetails provides a standard structure
            // for HTTP API errors.
            //
            // The response will contain information such as:
            //
            // status
            // title
            // detail
            //
            var problemDetails = new ProblemDetails
            {
                // HTTP status code.
                Status = StatusCodes.Status409Conflict,


                // Short description of the error.
                Title = "Conflict",


                // Detailed business error message.
                //
                // Example:
                //
                // "Email already exists."
                //
                // This comes from our custom exception.
                Detail = ex.Message
            };


            // ------------------------------------------------
            // CONVERT OBJECT TO JSON
            // ------------------------------------------------

            // ProblemDetails is a C# object.
            //
            // The client expects JSON.
            //
            // Therefore we serialize:
            //
            // C# Object
            //     ↓
            // JsonSerializer
            //     ↓
            // JSON
            //
            var json = JsonSerializer.Serialize(problemDetails);


            // ------------------------------------------------
            // WRITE RESPONSE
            // ------------------------------------------------

            // Send the JSON response back to the client.
            await context.Response.WriteAsync(json);
        }


        // ====================================================
        // HANDLE GENERAL EXCEPTION
        // ====================================================

        // Handles unexpected exceptions.
        //
        // Example:
        //
        // Database connection fails
        //        ↓
        // Exception
        //        ↓
        // This method
        //        ↓
        // 500 Internal Server Error
        private static async Task HandleExceptionAsync(
            HttpContext context)
        {
            // ------------------------------------------------
            // HTTP STATUS CODE
            // ------------------------------------------------

            // 500 means:
            //
            // "Something unexpected went wrong on the server."
            //
            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;


            // ------------------------------------------------
            // CONTENT TYPE
            // ------------------------------------------------

            // Tell the client that the response is
            // Problem Details JSON.
            context.Response.ContentType =
                "application/problem+json";


            // ------------------------------------------------
            // CREATE ERROR RESPONSE
            // ------------------------------------------------

            var problemDetails = new ProblemDetails
            {
                // HTTP status:
                //
                // 500 Internal Server Error
                Status = StatusCodes.Status500InternalServerError,


                // Short error title.
                Title = "Internal Server Error",


                // Safe message for the client.
                //
                // We do NOT expose:
                //
                // ex.Message
                //
                // because internal exception details may expose
                // sensitive information.
                Detail = "An unexpected error occurred."
            };


            // ------------------------------------------------
            // CONVERT TO JSON
            // ------------------------------------------------

            // Convert ProblemDetails object to JSON.
            var json = JsonSerializer.Serialize(problemDetails);


            // ------------------------------------------------
            // SEND RESPONSE
            // ------------------------------------------------

            // Write the JSON response to the HTTP response.
            await context.Response.WriteAsync(json);
        }
    }
}

