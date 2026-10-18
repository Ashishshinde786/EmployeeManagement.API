
// ============================================================
// ENTITY FRAMEWORK CORE MIGRATION
// ============================================================

// This namespace provides the Migration and MigrationBuilder
// classes used by Entity Framework Core.
//
// EF Core migrations allow us to track and apply database
// schema changes from our C# application.
using Microsoft.EntityFrameworkCore.Migrations;


// ============================================================
// NULLABLE CONTEXT
// ============================================================

// This disables nullable reference type warnings for this
// generated migration file.
//
// Migration files are normally generated automatically by
// Entity Framework Core.
#nullable disable


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.Migrations
{
    // ========================================================
    // ADD USERS MIGRATION
    // ========================================================

    // This migration is named "AddUsers".
    //
    // Its purpose is to create the Users table in the database.
    //
    // This migration would typically be generated after adding
    // a User entity and DbSet<User> to AppDbContext.
    //
    // Example command:
    //
    // Add-Migration AddUsers
    //
    // Then we apply it using:
    //
    // Update-Database
    //
    public partial class AddUsers : Migration
    {
        // ====================================================
        // UP METHOD
        // ====================================================

        // Up() contains the database changes that should be
        // applied when this migration is executed.
        //
        // When we run:
        //
        // Update-Database
        //
        // EF Core executes this method.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =================================================
            // CREATE USERS TABLE
            // =================================================

            // CreateTable() tells EF Core to create a new
            // database table named "Users".
            //
            // This table will store authentication/user
            // information for our application.
            migrationBuilder.CreateTable(
                name: "Users",

                // =============================================
                // TABLE COLUMNS
                // =============================================

                // This section defines the columns that will
                // exist in the Users table.
                columns: table => new
                {
                    // =========================================
                    // ID COLUMN
                    // =========================================

                    // Id uniquely identifies each user.
                    //
                    // C# type:
                    //
                    // int
                    //
                    // SQL Server type:
                    //
                    // int
                    //
                    // nullable: false
                    //
                    // means the Id cannot be NULL.
                    //
                    // Identity:
                    //
                    // "1, 1"
                    //
                    // means SQL Server automatically generates
                    // the ID.
                    //
                    // First value:
                    // 1
                    //
                    // Increment:
                    // 1
                    //
                    // Example:
                    //
                    // User 1 → Id = 1
                    // User 2 → Id = 2
                    // User 3 → Id = 3
                    //
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),


                    // =========================================
                    // USERNAME COLUMN
                    // =========================================

                    // Stores the username used for authentication.
                    //
                    // Example:
                    //
                    // "ashish"
                    //
                    // SQL Server type:
                    //
                    // nvarchar(max)
                    //
                    // nullable: false
                    //
                    // means Username cannot be NULL.
                    Username = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),


                    // =========================================
                    // PASSWORD COLUMN
                    // =========================================

                    // Stores the user's password value.
                    //
                    // IMPORTANT:
                    //
                    // Although this column is called "Password",
                    // the application should NOT store the user's
                    // plain-text password here.
                    //
                    // Your AuthService should hash the password
                    // using IPasswordHasher<User>.
                    //
                    // The database should contain the password
                    // HASH, not the original password.
                    //
                    // Example:
                    //
                    // User enters:
                    //
                    // "Password123"
                    //
                    // Database should store something similar
                    // to a generated password hash, not:
                    //
                    // "Password123"
                    //
                    Password = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),


                    // =========================================
                    // ROLE COLUMN
                    // =========================================

                    // Stores the user's role.
                    //
                    // Example:
                    //
                    // "Admin"
                    // "User"
                    //
                    // Your EmployeeController currently uses:
                    //
                    // [Authorize(Roles = "Admin")]
                    //
                    // for operations such as:
                    //
                    // POST
                    // PUT
                    // DELETE
                    //
                    // Therefore, this Role value is important
                    // for role-based authorization.
                    //
                    // nullable: false
                    //
                    // means every user must have a role.
                    Role = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false)
                },

                // =============================================
                // TABLE CONSTRAINTS
                // =============================================

                // Constraints define database rules.
                constraints: table =>
                {
                    // =========================================
                    // PRIMARY KEY
                    // =========================================

                    // Creates a primary key on the Id column.
                    //
                    // Constraint name:
                    //
                    // PK_Users
                    //
                    // The primary key ensures that every user
                    // has a unique identifier.
                    table.PrimaryKey(
                        "PK_Users",
                        x => x.Id);
                });
        }


        // ====================================================
        // DOWN METHOD
        // ====================================================

        // Down() contains the reverse operation of Up().
        //
        // Up():
        //
        //     Creates Users table
        //
        // Down():
        //
        //     Removes Users table
        //
        // EF Core uses this method when the migration needs
        // to be rolled back.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the Users table from the database.
            //
            // WARNING:
            //
            // Dropping the table also removes the user records
            // stored in that table.
            //
            // Therefore, rolling back migrations should be
            // handled carefully, especially in production.
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

