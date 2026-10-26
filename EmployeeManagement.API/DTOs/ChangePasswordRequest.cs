// Imports validation attributes such as [Required] and [StringLength].
// These attributes help ASP.NET Core validate incoming request data.
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    // DTO = Data Transfer Object.
    //
    // This class defines the structure of the data that the client
    // must send to the change-password API endpoint.
    //
    // It contains the current password for verification and the new
    // password that the user wants to set.
    //
    // This class only defines and validates the request data.
    // It does not verify passwords or update the database itself.
    public class ChangePasswordRequest
    {
        // [Required] indicates that the current password must be provided.
        //
        // ASP.NET Core model validation checks this attribute when
        // processing the incoming request body.
        //
        // The user enters this value in the frontend, Swagger, or Postman.
        // It is sent as part of the JSON request body.
        //
        // The AuthService later verifies this password against the
        // stored password hash.
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        // [Required] indicates that the new password must be provided.
        //
        // [StringLength(100, MinimumLength = 8)] specifies that the
        // password must contain between 8 and 100 characters.
        //
        // These are request-validation rules, not password hashing rules.
        // AuthService hashes the new password before saving it.
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}