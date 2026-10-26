using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.DTOs
{
    /// <summary>
    /// Represents the data required to register a new user.
    /// The client sends this data in the registration API request.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Gets or sets the username provided by the client.
        ///
        /// [Required] ensures that a value must be provided.
        /// [StringLength] allows a maximum of 50 characters
        /// and requires at least 3 characters.
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password provided by the client.
        ///
        /// [Required] ensures that a value must be provided.
        /// [StringLength] requires at least 8 characters
        /// and allows a maximum of 100 characters.
        ///
        /// This property contains the incoming password.
        /// The authentication service should hash the password
        /// before storing it in the database.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}