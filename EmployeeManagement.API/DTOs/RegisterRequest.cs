
// ============================================================
// IMPORT / NAMESPACE
// ============================================================

// System.ComponentModel.DataAnnotations provides validation
// attributes such as:
// [Required]
// [StringLength]
// [EmailAddress]
// [Range]
//
// We use these attributes to validate the registration data
// before it reaches our business logic.
using System.ComponentModel.DataAnnotations;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.DTOs
{
    // ========================================================
    // REGISTER REQUEST DTO
    // ========================================================

    // RegisterRequest is an INPUT DTO.
    //
    // It represents the data that the client sends when
    // creating/registering a new user account.
    //
    // Flow:
    //
    // Client
    //    ↓
    // RegisterRequest
    //    ↓
    // AuthController
    //    ↓
    // AuthService
    //    ↓
    // Password Hashing
    //    ↓
    // Database
    //
    // Example request:
    //
    // POST /api/auth/register
    //
    // {
    //     "username": "ashish",
    //     "password": "Password123"
    // }
    //
    public class RegisterRequest
    {
        // ====================================================
        // USERNAME
        // ====================================================

        // [Required]
        //
        // This means Username must be provided by the client.
        //
        // If the client doesn't provide a username,
        // model validation will fail.
        //
        // Because your controller uses [ApiController],
        // ASP.NET Core can automatically return:
        //
        // 400 Bad Request
        //
        [Required]


        // [StringLength(50, MinimumLength = 3)]
        //
        // This applies length validation to the username.
        //
        // Maximum length:
        //     50 characters
        //
        // Minimum length:
        //     3 characters
        //
        // Valid examples:
        //
        // "ashish"
        // "john123"
        // "developer"
        //
        // Invalid examples:
        //
        // "ab"          → Less than 3 characters
        // "very-long..." → More than 50 characters
        //
        [StringLength(50, MinimumLength = 3)]


        // Username entered by the user during registration.
        //
        // Example:
        //
        // "ashish"
        //
        // = string.Empty initializes the property with an
        // empty string instead of null.
        //
        // IMPORTANT:
        //
        // string.Empty does NOT perform validation.
        //
        // [Required] is responsible for validation.
        public string Username { get; set; } = string.Empty;


        // ====================================================
        // PASSWORD
        // ====================================================

        // [Required]
        //
        // Password must be provided by the client.
        //
        // If Password is missing, validation fails.
        [Required]


        // [StringLength(100, MinimumLength = 6)]
        //
        // This validates the password length.
        //
        // Minimum password length:
        //     6 characters
        //
        // Maximum password length:
        //     100 characters
        //
        // Example of a valid password:
        //
        // "Password123"
        //
        // Example of an invalid password:
        //
        // "abc"
        //
        // because it contains fewer than 6 characters.
        //
        // NOTE:
        //
        // This only checks the length.
        //
        // It does NOT check whether the password contains:
        //
        // - uppercase letters
        // - lowercase letters
        // - numbers
        // - special characters
        //
        // If your business requires stronger password rules,
        // you can add additional validation later.
        [StringLength(100, MinimumLength = 6)]


        // Password supplied by the client during registration.
        //
        // Example:
        //
        // "Password123"
        //
        // IMPORTANT SECURITY POINT:
        //
        // This password should NOT be stored directly in the
        // database.
        //
        // The AuthService should hash the password using:
        //
        // IPasswordHasher<User>
        //
        // The database should store the password HASH instead
        // of the original plain-text password.
        public string Password { get; set; } = string.Empty;
    }
}

