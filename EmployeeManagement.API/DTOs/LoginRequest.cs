// Imports validation attributes such as [Required].
// These attributes help ASP.NET Core validate incoming request data.
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    // LoginRequest defines the structure of the data required
    // to log in to the application.
    //
    // The client sends this data in the HTTP request body.
    // ASP.NET Core converts the JSON into a LoginRequest object.
    //
    // This class only represents and validates the request data.
    // It does not verify credentials or generate a JWT.
    public class LoginRequest
    {
        // The username entered by the user.
        //
        // [Required] tells model validation that this field is mandatory.
        //
        // The value normally comes from the login form in the frontend,
        // or from Swagger or Postman during testing.
        //
        // AuthService uses this value to find the matching user
        // in the database.
        [Required]
        public string Username { get; set; } = string.Empty;

        // The password entered by the user.
        //
        // [Required] tells model validation that this field is mandatory.
        //
        // AuthService verifies this supplied password against the
        // stored password hash using IPasswordHasher<User>.
        //
        // The plain-text password should only be handled as needed
        // for verification and must never be stored as plain text.
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}