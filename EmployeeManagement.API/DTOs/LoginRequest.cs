
// ============================================================
// NAMESPACE / IMPORT
// ============================================================

// System.ComponentModel.DataAnnotations provides validation
// attributes such as:
//
// [Required]
// [StringLength]
// [EmailAddress]
// [Range]
//
// We use [Required] in this DTO to make sure the client
// provides both Username and Password.
using System.ComponentModel.DataAnnotations;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.DTOs
{
    // ========================================================
    // LOGIN REQUEST DTO
    // ========================================================

    // LoginRequest is a DTO used to receive login credentials
    // from the client.
    //
    // DTO = Data Transfer Object.
    //
    // This is an INPUT DTO because the direction is:
    //
    // Client
    //    ↓
    // LoginRequest
    //    ↓
    // AuthController
    //    ↓
    // AuthService
    //
    // Example HTTP request:
    //
    // POST /api/auth/login
    //
    // {
    //     "username": "ashish",
    //     "password": "Password123"
    // }
    //
    public class LoginRequest
    {
        // ====================================================
        // USERNAME
        // ====================================================

        // [Required] means the client must provide
        // a Username value.
        //
        // If Username is missing or invalid according to
        // the Required validator, model validation fails.
        //
        // Because AuthController has [ApiController],
        // ASP.NET Core can automatically return:
        //
        // 400 Bad Request
        //
        // when model validation fails.
        [Required]


        // Username supplied by the user during login.
        //
        // Example:
        //
        // "ashish"
        //
        // = string.Empty means the property starts with an
        // empty string instead of null.
        public string Username { get; set; } = string.Empty;


        // ====================================================
        // PASSWORD
        // ====================================================

        // Password is also required.
        //
        // The client must provide a password.
        //
        // If the password is missing, validation fails.
        [Required]


        // Password supplied by the user.
        //
        // IMPORTANT:
        //
        // The password should be sent over HTTPS so that
        // credentials are encrypted while traveling between
        // client and server.
        //
        // The application should NOT store this plain-text
        // password in the database.
        //
        // During authentication, AuthService should compare
        // the supplied password against the stored password
        // hash using IPasswordHasher<User>.
        public string Password { get; set; } = string.Empty;
    }
}

