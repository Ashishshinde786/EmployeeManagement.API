
// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains the model/entity classes used by
// the Employee Management API.
//
// Models represent the data objects used by our application.
//
// The User model represents a user who can register, log in,
// and access protected API endpoints.
namespace EmployeeManagement.API.Models
{
    // ========================================================
    // USER ENTITY / MODEL
    // ========================================================

    // User represents a user of our application.
    //
    // This is an Entity class because Entity Framework Core
    // uses this class to map user objects to the "Users"
    // table in the SQL Server database.
    //
    // Database mapping:
    //
    // User class
    //      ↓
    // Entity Framework Core
    //      ↓
    // Users table
    //
    public class User
    {
        // ====================================================
        // ID
        // ====================================================

        // Unique identifier for each user.
        //
        // This property is normally used as the Primary Key
        // of the Users table.
        //
        // Example:
        //
        // User 1 → Id = 1
        // User 2 → Id = 2
        // User 3 → Id = 3
        //
        // In SQL Server, EF Core configures this integer
        // property as an Identity column, so the database
        // automatically generates the value when a new user
        // is inserted.
        public int Id { get; set; }


        // ====================================================
        // USERNAME
        // ====================================================

        // Stores the username of the application user.
        //
        // Example:
        //
        // "ashish"
        //
        // This username is used during login to identify
        // the user.
        //
        // The username comes from the client through DTOs
        // such as:
        //
        // RegisterRequest
        // LoginRequest
        //
        // string.Empty initializes the property with an empty
        // string instead of null.
        //
        // The actual validation rules for registration are
        // defined in RegisterRequest.
        public string Username { get; set; } = string.Empty;


        // ====================================================
        // PASSWORD
        // ====================================================

        // Stores the user's password value.
        //
        // IMPORTANT:
        //
        // This property should contain a PASSWORD HASH,
        // not the user's original plain-text password.
        //
        // Registration flow:
        //
        // User enters password
        //          ↓
        // RegisterRequest
        //          ↓
        // AuthService
        //          ↓
        // PasswordHasher<User>
        //          ↓
        // Password Hash
        //          ↓
        // User.Password
        //          ↓
        // Database
        //
        // During login, the entered password is verified
        // against this stored password hash.
        //
        // Your application uses IPasswordHasher<User> for
        // password hashing and verification.
        public string Password { get; set; } = string.Empty;


        // ====================================================
        // ROLE
        // ====================================================

        // Stores the authorization role assigned to the user.
        //
        // Examples:
        //
        // "Admin"
        // "User"
        //
        // The role is important for Role-Based Authorization.
        //
        // For example, your EmployeeController contains:
        //
        // [Authorize(Roles = "Admin")]
        //
        // This means only authenticated users whose role is
        // "Admin" can perform certain operations such as:
        //
        // POST   /api/employees
        // PUT    /api/employees/{id}
        // DELETE /api/employees/{id}
        //
        // The role can also be included as a claim inside the
        // JWT token during login.
        public string Role { get; set; } = string.Empty;
    }
}
