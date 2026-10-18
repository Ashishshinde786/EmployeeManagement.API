
// ============================================================
// REQUIRED NAMESPACES
// ============================================================

// Provides access to AppDbContext.
//
// AppDbContext is our Entity Framework Core database context
// and gives us access to the Users table.
using EmployeeManagement.API.Data;


// Provides access to DTO classes such as:
//
// - RegisterRequest
// - LoginRequest
//
// DTOs are used to receive data from the client/API request.
using EmployeeManagement.API.DTOs;


// Provides access to our custom exceptions.
//
// AuthService uses ConflictException when a username already
// exists in the database.
using EmployeeManagement.API.Exceptions;


// Provides access to the User entity/model.
//
// The User model represents a user stored in the database.
using EmployeeManagement.API.Models;


// Provides ASP.NET Core Identity password hashing functionality.
//
// We use IPasswordHasher<User> to:
//
// - Hash passwords during registration
// - Verify passwords during login
//
// This prevents us from storing plain-text passwords.
using Microsoft.AspNetCore.Identity;


// Provides Entity Framework Core methods such as:
//
// - FirstOrDefaultAsync()
//
// These methods allow us to query the database asynchronously.
using Microsoft.EntityFrameworkCore;


// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains service classes.
//
// The Service layer contains application/business logic.
//
// Architecture:
//
// Controller
//     ↓
// AuthService
//     ↓
// AppDbContext
//     ↓
// SQL Server
//
// AuthService is responsible for authentication-related
// business logic such as registration and login.
namespace EmployeeManagement.API.Services
{
    // ========================================================
    // AUTH SERVICE
    // ========================================================

    // AuthService implements IAuthService.
    //
    // IAuthService defines the authentication operations
    // that this service must provide.
    //
    // AuthService contains the actual implementation for:
    //
    // - User registration
    // - Password hashing
    // - User login
    // - Password verification
    // - JWT token generation
    public class AuthService : IAuthService
    {
        // ====================================================
        // DATABASE CONTEXT
        // ====================================================

        // _context is our Entity Framework Core database context.
        //
        // It gives us access to the Users table through:
        //
        // _context.Users
        //
        // We use it to:
        //
        // - Check whether a username already exists
        // - Find a user during login
        // - Add a new user
        // - Save changes to the database
        private readonly AppDbContext _context;


        // ====================================================
        // PASSWORD HASHER
        // ====================================================

        // IPasswordHasher<User> is provided by ASP.NET Core
        // Identity.
        //
        // It is responsible for securely hashing and verifying
        // user passwords.
        //
        // During registration:
        //
        // Plain password
        //       ↓
        // HashPassword()
        //       ↓
        // Password hash
        //       ↓
        // Database
        //
        // During login:
        //
        // Entered password
        //       ↓
        // VerifyHashedPassword()
        //       ↓
        // Match / Failed
        private readonly IPasswordHasher<User> _passwordHasher;


        // ====================================================
        // JWT SERVICE
        // ====================================================

        // IJwtService is responsible for generating JWT tokens.
        //
        // After successful login, AuthService passes the
        // authenticated User object to this service.
        //
        // The JWT can then be returned to the client and used
        // for authenticated API requests.
        private readonly IJwtService _jwtService;


        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // Dependencies are provided through Dependency Injection.
        //
        // ASP.NET Core creates these objects and passes them
        // into AuthService.
        //
        // Required dependencies:
        //
        // AppDbContext
        //     → Database access
        //
        // IPasswordHasher<User>
        //     → Password hashing and verification
        //
        // IJwtService
        //     → JWT token generation
        public AuthService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            // Store the injected database context.
            _context = context;

            // Store the injected password hashing service.
            _passwordHasher = passwordHasher;

            // Store the injected JWT service.
            _jwtService = jwtService;
        }


        // ============================================================
        // REGISTER USER
        // ============================================================

        // This method registers a new user in the application.
        //
        // Registration flow:
        //
        // Client
        //   ↓
        // POST /api/auth/register
        //   ↓
        // AuthController
        //   ↓
        // AuthService
        //   ↓
        // Check username
        //   ↓
        // Create User
        //   ↓
        // Hash password
        //   ↓
        // Save User
        //   ↓
        // Database
        //
        // The method returns a message after successful
        // registration.
        public async Task<string> RegisterAsync(
            RegisterRequest request)
        {
            // =================================================
            // CHECK EXISTING USERNAME
            // =================================================

            // Search the Users table for a user whose username
            // matches the username provided in the request.
            //
            // FirstOrDefaultAsync() returns:
            //
            // - The matching User if found
            // - null if no matching user exists
            //
            // This check prevents duplicate usernames.
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == request.Username);


            // =================================================
            // HANDLE DUPLICATE USERNAME
            // =================================================

            // If a matching user was found, the username is
            // already registered.
            if (existingUser != null)
            {
                // This is a business conflict because the
                // requested username cannot be used again.
                //
                // ConflictException is later handled by the
                // GlobalExceptionMiddleware.
                //
                // Middleware converts this exception into:
                //
                // HTTP 409 Conflict
                throw new ConflictException(
                    "Username already exists.");
            }


            // =================================================
            // CREATE USER OBJECT
            // =================================================

            // Create a new User entity.
            //
            // At this point, we are creating the C# object that
            // will eventually be stored in the Users table.
            var user = new User
            {
                // Store the username received from the request.
                Username = request.Username,

                // Assign the default role as "User".
                //
                // This means a user registering through the
                // public registration API does not automatically
                // become an administrator.
                //
                // Admin access can be handled separately.
                Role = "User"
            };


            // =================================================
            // HASH PASSWORD
            // =================================================

            // NEVER store the user's plain-text password
            // directly in the database.
            //
            // request.Password contains the password entered
            // by the user.
            //
            // HashPassword() converts the password into a
            // password hash.
            //
            // The resulting hash is stored in User.Password.
            //
            // Example conceptually:
            //
            // User enters:
            //     MyPassword123
            //
            // Stored in database:
            //     Password hash
            //
            // The original plain-text password is not stored.
            user.Password = _passwordHasher.HashPassword(
                user,
                request.Password);


            // =================================================
            // ADD USER TO EF CORE CONTEXT
            // =================================================

            // Add the new User entity to the Users DbSet.
            //
            // EF Core now tracks this entity as "Added".
            //
            // The INSERT statement will be generated when
            // SaveChangesAsync() is called.
            _context.Users.Add(user);


            // =================================================
            // SAVE USER TO DATABASE
            // =================================================

            // Persist the pending changes to SQL Server.
            //
            // At this point EF Core sends an INSERT statement
            // to the database.
            //
            // The new User record is now stored in the Users
            // table.
            await _context.SaveChangesAsync();


            // =================================================
            // RETURN SUCCESS MESSAGE
            // =================================================

            // Registration completed successfully.
            return "User registered successfully.";
        }


        // ============================================================
        // LOGIN USER
        // ============================================================

        // This method authenticates an existing user.
        //
        // Login flow:
        //
        // Client
        //   ↓
        // POST /api/auth/login
        //   ↓
        // AuthController
        //   ↓
        // AuthService
        //   ↓
        // Find user
        //   ↓
        // Verify password
        //   ↓
        // Generate JWT
        //   ↓
        // Return token
        //
        // string? means the method can return:
        //
        // string → successful login with JWT
        //
        // null → invalid username/password
        public async Task<string?> LoginAsync(
            LoginRequest request)
        {
            // =================================================
            // FIND USER
            // =================================================

            // Search the Users table using the username
            // provided by the client.
            //
            // If the username exists:
            //
            //     user contains the User entity.
            //
            // If the username does not exist:
            //
            //     user is null.
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == request.Username);


            // =================================================
            // USER NOT FOUND
            // =================================================

            // If no user exists with the provided username,
            // authentication fails.
            if (user == null)
            {
                // Return null to indicate that authentication
                // was not successful.
                //
                // AuthController receives null and returns:
                //
                // HTTP 401 Unauthorized
                return null;
            }


            // =================================================
            // VERIFY PASSWORD
            // =================================================

            // Compare the password entered by the user with
            // the password hash stored in the database.
            //
            // Parameters:
            //
            // user
            //     → User object associated with the hash.
            //
            // user.Password
            //     → Hashed password stored in database.
            //
            // request.Password
            //     → Plain password entered during login.
            //
            // The password hasher performs the secure
            // verification process.
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password);


            // =================================================
            // INVALID PASSWORD
            // =================================================

            // If the password verification fails, authentication
            // is unsuccessful.
            if (result == PasswordVerificationResult.Failed)
            {
                // Return null.
                //
                // AuthController will return:
                //
                // HTTP 401 Unauthorized
                return null;
            }


            // =================================================
            // GENERATE JWT TOKEN
            // =================================================

            // At this point:
            //
            // 1. Username exists.
            // 2. Password is correct.
            //
            // Therefore authentication was successful.
            //
            // Generate a JWT token for the authenticated user.
            //
            // The JWT service can include information such as:
            //
            // - User ID
            // - Username
            // - Role
            //
            // The client will use this token when calling
            // protected APIs.
            var token = _jwtService.GenerateToken(user);


            // =================================================
            // RETURN JWT TOKEN
            // =================================================

            // Return the generated token to AuthController.
            //
            // AuthController returns it to the client as:
            //
            // {
            //     "token": "eyJhbGciOi..."
            // }
            return token;
        }
    }
}

