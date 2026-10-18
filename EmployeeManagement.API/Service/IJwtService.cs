
// ============================================================
// REQUIRED NAMESPACE
// ============================================================

// Provides access to the User entity/model.
//
// IJwtService needs the User object because the JWT token
// is generated based on information about the authenticated
// user, such as:
// - User ID
// - Username
// - Role
using EmployeeManagement.API.Models;


// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains service classes and interfaces.
//
// JWT-related business/service logic is kept inside the
// Services layer.
//
// Authentication flow:
//
// AuthController
//      ↓
// AuthService
//      ↓
// IJwtService
//      ↓
// JwtService
//      ↓
// JWT Token
namespace EmployeeManagement.API.Services
{
    // ========================================================
    // JWT SERVICE INTERFACE
    // ========================================================

    // IJwtService defines the contract for JWT token generation.
    //
    // It tells the application that any JWT service
    // implementation must provide a method for generating
    // a token for a User.
    //
    // The actual implementation is provided by:
    //
    // JwtService : IJwtService
    //
    // Why use an interface?
    //
    // - Loose coupling
    // - Dependency Injection
    // - Easier unit testing
    // - Separation of responsibilities
    //
    // AuthService does not need to know HOW the JWT token
    // is generated.
    //
    // It only needs to know that it can call:
    //
    // GenerateToken(user)
    public interface IJwtService
    {
        // ====================================================
        // GENERATE JWT TOKEN
        // ====================================================

        // Generates a JSON Web Token (JWT) for the specified
        // authenticated user.
        //
        // The User object contains information that can be
        // included in the JWT claims.
        //
        // For example:
        //
        // User
        // ├── Id
        // ├── Username
        // └── Role
        //
        // The JwtService implementation can use this
        // information to create claims such as:
        //
        // - User ID
        // - Username
        // - Role
        //
        // After successful login:
        //
        // AuthService
        //      ↓
        // _jwtService.GenerateToken(user)
        //      ↓
        // JWT token
        //      ↓
        // AuthController
        //      ↓
        // Client
        //
        // The client can then send the JWT in the Authorization
        // header when calling protected APIs:
        //
        // Authorization: Bearer <token>
        //
        // string is returned because a JWT token is represented
        // as a string.
        string GenerateToken(User user);
    }
}

