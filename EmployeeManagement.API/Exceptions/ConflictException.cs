namespace EmployeeManagement.API.Exceptions
{
    // Custom exception for conflict situations in the application.
    // Inheriting from Exception allows this class to be thrown and caught
    // like other standard C# exceptions.
    public class ConflictException : Exception
    {
        // Accepts an error message from the code that creates the exception.
        // The ": base(message)" part passes that message to the parent
        // Exception class so it is available through the Message property.
        public ConflictException(string message) : base(message)
        {
            // No additional logic is required here.
            // The base Exception class stores the supplied message.
        }
    }
}