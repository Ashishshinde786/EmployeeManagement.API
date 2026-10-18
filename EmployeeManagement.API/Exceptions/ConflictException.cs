
// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains custom exception classes used by
// the Employee Management API.
//
// Keeping custom exceptions in a separate "Exceptions"
// folder makes the project easier to organize and maintain.
namespace EmployeeManagement.API.Exceptions
{
    // ========================================================
    // CUSTOM CONFLICT EXCEPTION
    // ========================================================

    // ConflictException is a custom exception.
    //
    // It inherits from the built-in .NET Exception class.
    //
    // We create a custom exception because we want to
    // specifically identify business conflicts in our
    // application.
    //
    // Example:
    //
    // Suppose a user tries to register with a username
    // that already exists.
    //
    // AuthService can throw:
    //
    // throw new ConflictException(
    //     "Username already exists.");
    //
    // Our GlobalExceptionMiddleware catches this exception
    // and converts it into an HTTP 409 Conflict response.
    //
    public class ConflictException : Exception
    {
        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // This constructor receives a message describing
        // the conflict.
        //
        // Example:
        //
        // new ConflictException(
        //     "Username already exists.");
        //
        // The message is passed to the base Exception class
        // using:
        //
        // : base(message)
        //
        // This allows us to access the message later using:
        //
        // ex.Message
        //
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}

