// Imports the Employee and User entity classes.
// These classes describe the data that our application stores.
using EmployeeManagement.API.Models;

// Imports Entity Framework Core.
// DbContext, DbSet, ModelBuilder, and database configuration
// functionality come from this library.
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Data
{
    // AppDbContext represents a session with the database.
    //
    // It tracks database entities when tracking is enabled,
    // translates LINQ queries into SQL, and saves changes
    // made to tracked entities.
    //
    // Inheriting from DbContext gives this class EF Core's
    // database-access functionality.
    public class AppDbContext : DbContext
    {
        // Constructor called by ASP.NET Core's dependency injection.
        //
        // DbContextOptions<AppDbContext> contains the configuration
        // for this particular context, including the database provider
        // and other EF Core settings.
        //
        // The configuration is registered in Program.cs.
        public AppDbContext(DbContextOptions<AppDbContext> options)

            // Pass the options to the base DbContext class so EF Core
            // can initialize the context using that configuration.
            : base(options)
        {
            // No additional initialization is required here.
        }

        // Represents the collection of Employee entities that
        // EF Core can query and modify.
        //
        // Set<Employee>() returns the DbSet associated with Employee.
        // EF Core maps this entity to its database table according
        // to the model configuration and migrations.
        //
        // Example:
        // _context.Employees.ToListAsync()
        // retrieves employee records from the database.
        public DbSet<Employee> Employees => Set<Employee>();

        // Represents the collection of User entities.
        //
        // This set is used for registration, login, and other
        // operations involving application users.
        public DbSet<User> Users => Set<User>();

        // EF Core calls this method while building the model that
        // describes how entities map to database tables and columns.
        //
        // This method configures database mapping rules using
        // Fluent API. Migrations can use these rules to create
        // or change the database schema.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Employee entity and its properties.
            modelBuilder.Entity<Employee>(entity =>
            {
                // The Name column must not be NULL and can contain
                // a maximum of 100 characters.
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // The Email column must not be NULL and can contain
                // a maximum of 256 characters.
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                // The Department column must not be NULL and can
                // contain a maximum of 100 characters.
                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configure Salary as a decimal value with precision 18
                // and scale 2.
                //
                // Precision 18 means up to 18 total digits.
                // Scale 2 means up to 2 digits after the decimal point.
                //
                // Example: 1234567890123456.78
                entity.Property(e => e.Salary)
                    .HasPrecision(18, 2);
            });

            // Configure the User entity and its properties.
            modelBuilder.Entity<User>(entity =>
            {
                // Username is required and limited to 50 characters.
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                // Password is required.
                //
                // In this application, the AuthService stores a password
                // hash in this property, not the plain-text password.
                entity.Property(u => u.Password)
                    .IsRequired();

                // Role is required and limited to 20 characters.
                // Examples in this application: "Admin" and "User".
                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(20);

                // Create a unique database index on Username.
                //
                // The database will reject duplicate usernames,
                // even if two registration requests attempt to create
                // the same username at nearly the same time.
                entity.HasIndex(u => u.Username)
                    .IsUnique();
            });
        }
    }
}