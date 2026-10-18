
// ============================================================
// REQUIRED NAMESPACE
// ============================================================

// Provides access to authentication-related DTOs.
//
// This interface uses:
// - RegisterRequest
// - LoginRequest
//
// These DTOs contain the data received from the client
// during registration and login.
using EmployeeManagement.API.DTOs;


// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains service classes and interfaces.
//
// The Service layer contains application/business logic.
//
// Authentication flow:
//
// AuthController
//      ↓
// IAuthService
//      ↓
// AuthService
//      ↓
// AppDbContext / JWT Service
namespace EmployeeManagement.API.Services
{
    // ========================================================
    // AUTHENTICATION SERVICE INTERFACE
    // ========================================================

    // IAuthService defines the contract for authentication
    // operations in the application.
    //
    // It specifies WHAT authentication operations are
    // available, but does not contain the actual implementation.
    //
    // The implementation is provided by:
    //
    // AuthService : IAuthService
    //
    // Using an interface gives us:
    //
    // - Loose coupling
    // - Dependency Injection
    // - Easier unit testing
    // - Separation of responsibilities
    public interface IAuthService
    {
        // ====================================================
        // REGISTER USER
        // ====================================================

        // Registers a new user in the application.
        //
        // RegisterRequest contains the information provided
        // by the client during registration.
        //
        // For example:
        //
        // {
        //     "username": "ashish",
        //     "password": "Password123"
        // }
        //
        // The AuthService implementation performs operations
        // such as:
        //
        // 1. Check whether username already exists.
        // 2. Create a User entity.
        // 3. Hash the password.
        // 4. Assign the default User role.
        // 5. Save the user to the database.
        //
        // Task<string> means this method is asynchronous and
        // returns a string after the registration operation
        // completes.
        //
        // Example successful result:
        //
        // "User registered successfully."
        Task<string> RegisterAsync(RegisterRequest request);


        // ====================================================
        // LOGIN USER
        // ====================================================

        // Authenticates an existing user.
        //
        // LoginRequest contains:
        //
        // - Username
        // - Password
        //
        // The AuthService implementation:
        //
        // 1. Finds the user by username.
        // 2. Verifies the entered password against the
        //    stored password hash.
        // 3. Generates a JWT token if authentication succeeds.
        //
        // Task<string?> means:
        //
        // string
        //     → Login succeeded and a JWT token is returned.
        //
        // null
        //     → Authentication failed.
        //
        // For example, invalid username or password can result
        // in null, which the Controller converts into:
        //
        // HTTP 401 Unauthorized
        Task<string?> LoginAsync(LoginRequest request);
    }
}

