
// ============================================================
// NAMESPACES / IMPORTS
// ============================================================

// Contains our database entity models.
//
// Employee
// User
//
// These classes represent tables in our database.
using EmployeeManagement.API.Models;


// Provides Entity Framework Core functionality.
//
// DbContext
// DbSet
// DbContextOptions
// ModelBuilder
//
// and other EF Core classes come from this namespace.
using Microsoft.EntityFrameworkCore;


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.Data
{
    // ========================================================
    // APP DB CONTEXT
    // ========================================================

    // DbContext is the main class used by Entity Framework Core
    // to communicate with the database.
    //
    // You can think of AppDbContext as a bridge between:
    //
    // C# Application
    //       ↓
    // AppDbContext
    //       ↓
    // Entity Framework Core
    //       ↓
    // SQL Server
    //
    // It is responsible for:
    //
    // - Managing database connections
    // - Tracking entities
    // - Querying data
    // - Saving changes
    // - Mapping C# classes to database tables
    // - Applying model configuration
    //
    public class AppDbContext : DbContext
    {
        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // DbContextOptions<AppDbContext> contains configuration
        // required by Entity Framework Core.
        //
        // For example, in Program.cs we configured:
        //
        // builder.Services.AddDbContext<AppDbContext>(options =>
        //     options.UseSqlServer(
        //         builder.Configuration
        //             .GetConnectionString("DefaultConnection")
        //     ));
        //
        // That configuration is passed into this constructor.
        //
        // Dependency Injection creates AppDbContext and provides
        // these options automatically.
        public AppDbContext(
            DbContextOptions<AppDbContext> options)

            // Pass the options to the base DbContext class.
            //
            // The base DbContext uses these options to know:
            //
            // - Which database provider to use
            // - Which connection configuration to use
            // - Other EF Core settings
            : base(options)
        {
        }


        // ====================================================
        // EMPLOYEE DBSET
        // ====================================================

        // DbSet<Employee> represents the Employee collection
        // managed by Entity Framework Core.
        //
        // In a typical relational database mapping:
        //
        // Employee C# class
        //        ↓
        // Employees table
        //
        // We can use:
        //
        // _context.Employees
        //
        // to query employees.
        //
        // Example:
        //
        // var employees =
        //     await _context.Employees.ToListAsync();
        //
        // EF Core converts this LINQ query into SQL.
        public DbSet<Employee> Employees { get; set; }


        // ====================================================
        // USER DBSET
        // ====================================================

        // Represents the Users table.
        //
        // User C# class
        //       ↓
        // Users table
        //
        // We can use:
        //
        // _context.Users
        //
        // to query or modify users.
        //
        // Example:
        //
        // var user =
        //     await _context.Users
        //         .FirstOrDefaultAsync(...);
        //
        public DbSet<User> Users { get; set; }


        // ====================================================
        // MODEL CONFIGURATION
        // ====================================================

        // OnModelCreating is called by Entity Framework Core
        // when it builds the database model.
        //
        // We use this method when we want to customize how
        // our C# classes and properties map to database tables
        // and columns.
        //
        // Examples of configurations we could define here:
        //
        // - Column precision
        // - Maximum string length
        // - Required fields
        // - Relationships
        // - Foreign keys
        // - Indexes
        // - Table names
        // - Default values
        //
        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            // =================================================
            // EMPLOYEE SALARY CONFIGURATION
            // =================================================

            // Configure the Salary property of Employee.
            //
            // e represents an Employee entity.
            //
            // e => e.Salary
            //
            // means:
            //
            // "Select the Salary property from Employee."
            //
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)


                // =================================================
                // DECIMAL PRECISION
                // =================================================

                // Configure the precision of the Salary column.
                //
                // HasPrecision(18, 2)
                //
                // means:
                //
                // Total digits = 18
                // Digits after decimal = 2
                //
                // Example:
                //
                // 9999999999999999.99
                //
                // The exact maximum depends on the database
                // provider/type, but conceptually:
                //
                // 18 total digits
                // 2 decimal places
                //
                // This is appropriate for monetary values because
                // salary normally needs decimal precision.
                .HasPrecision(18, 2);
        }
    }
}

