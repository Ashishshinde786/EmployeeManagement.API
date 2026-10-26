namespace EmployeeManagement.API.Models
{
    /// <summary>
    /// Represents a registered user account in the application.
    /// Entity Framework Core uses this class to map user data
    /// to the database.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for each registered user.
        /// EF Core normally recognizes Id as the primary key by convention.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username used to identify the user during login.
        /// string.Empty initializes the property with an empty string.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Stores the hashed password, never the plain-text password.
        /// During registration, the password hasher generates this value.
        /// During login, the hasher verifies the entered password
        /// against the stored hash.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Defines the user's role in the application.
        /// For example, Admin or User.
        /// Roles can be used by authorization rules to control
        /// which API endpoints the user can access.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}