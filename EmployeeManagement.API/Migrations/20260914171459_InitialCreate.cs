
// ============================================================
// ENTITY FRAMEWORK CORE MIGRATION
// ============================================================

// Migration classes are provided by Entity Framework Core.
//
// EF Core migrations are used to keep the database schema
// synchronized with the C# entity/model structure.
//
// For example, when we run:
//
// Add-Migration InitialCreate
//
// EF Core looks at the current model and generates a migration
// containing database changes.
//
// Then when we run:
//
// Update-Database
//
// EF Core executes this migration against the configured
// SQL Server database.
using Microsoft.EntityFrameworkCore.Migrations;


// ============================================================
// NULLABLE CONTEXT
// ============================================================

// This disables nullable reference type warnings for this
// generated migration file.
//
// EF Core generates migration files automatically, so normally
// we should not manually change this setting.
#nullable disable


// ============================================================
// NAMESPACE
// ============================================================

namespace EmployeeManagement.API.Migrations
{
    // ========================================================
    // INITIAL CREATE MIGRATION
    // ========================================================

    // This class represents the first migration of our
    // Employee Management API database.
    //
    // Migration name:
    //
    // InitialCreate
    //
    // It inherits from EF Core's Migration class.
    //
    // EF Core uses two main methods:
    //
    // Up()
    //   → Applies the database changes.
    //
    // Down()
    //   → Reverses/undoes the database changes.
    //
    public partial class InitialCreate : Migration
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
        // EF Core executes Up().
        //
        // In this migration, Up() creates the Employees table.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =================================================
            // CREATE EMPLOYEES TABLE
            // =================================================

            // CreateTable() tells EF Core to create a new
            // database table.
            //
            // The table name will be:
            //
            // Employees
            //
            // The "columns" section defines the columns inside
            // that table.
            //
            // The "constraints" section defines things such as
            // primary keys and foreign keys.
            migrationBuilder.CreateTable(
                name: "Employees",

                // =============================================
                // TABLE COLUMNS
                // =============================================

                // Here we define all columns that will exist
                // inside the Employees table.
                columns: table => new
                {
                    // =========================================
                    // ID COLUMN
                    // =========================================

                    // Id is the primary identifier of an
                    // employee.
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
                    // Annotation("SqlServer:Identity", "1, 1")
                    //
                    // means SQL Server will automatically
                    // generate the ID value.
                    //
                    // "1, 1" means:
                    //
                    // Seed     = 1
                    // Increment = 1
                    //
                    // So IDs will normally look like:
                    //
                    // 1
                    // 2
                    // 3
                    // 4
                    // ...
                    //
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),


                    // =========================================
                    // NAME COLUMN
                    // =========================================

                    // Stores the employee's name.
                    //
                    // C# type:
                    //
                    // string
                    //
                    // SQL Server type:
                    //
                    // nvarchar(max)
                    //
                    // nullable: false
                    //
                    // means the database does not allow NULL
                    // values in this column.
                    Name = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),


                    // =========================================
                    // EMAIL COLUMN
                    // =========================================

                    // Stores the employee's email address.
                    //
                    // Example:
                    //
                    // ashish@example.com
                    //
                    // SQL Server type:
                    //
                    // nvarchar(max)
                    //
                    // nullable: false
                    //
                    // means Email is required at the database
                    // level.
                    Email = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),


                    // =========================================
                    // DEPARTMENT COLUMN
                    // =========================================

                    // Stores the department of the employee.
                    //
                    // Example:
                    //
                    // IT
                    // HR
                    // Finance
                    // Operations
                    //
                    // SQL Server type:
                    //
                    // nvarchar(max)
                    //
                    // nullable: false
                    Department = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),


                    // =========================================
                    // SALARY COLUMN
                    // =========================================

                    // Stores the employee's salary.
                    //
                    // C# type:
                    //
                    // decimal
                    //
                    // SQL Server type:
                    //
                    // decimal(18,2)
                    //
                    // precision = 18
                    // scale     = 2
                    //
                    // This means:
                    //
                    // Maximum total digits = 18
                    // Digits after decimal = 2
                    //
                    // Example:
                    //
                    // 85000.50
                    //
                    // is valid.
                    //
                    // This configuration comes from your
                    // AppDbContext:
                    //
                    // .HasPrecision(18, 2)
                    //
                    // nullable: false
                    //
                    // means Salary cannot be NULL.
                    Salary = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false)
                },

                // =============================================
                // TABLE CONSTRAINTS
                // =============================================

                // Constraints define database rules such as
                // primary keys and relationships.
                constraints: table =>
                {
                    // =========================================
                    // PRIMARY KEY
                    // =========================================

                    // This creates a primary key on the Id
                    // column.
                    //
                    // Database constraint name:
                    //
                    // PK_Employees
                    //
                    // x => x.Id means the Id property is the
                    // primary key.
                    //
                    // A primary key uniquely identifies each
                    // employee record.
                    table.PrimaryKey(
                        "PK_Employees",
                        x => x.Id);
                });
        }


        // ====================================================
        // DOWN METHOD
        // ====================================================

        // Down() contains the reverse operation of Up().
        //
        // If Up() creates something:
        //
        // Down() removes it.
        //
        // In our case:
        //
        // Up()
        //   → Creates Employees table
        //
        // Down()
        //   → Deletes Employees table
        //
        // This allows EF Core to roll back a migration.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the Employees table from the database.
            //
            // IMPORTANT:
            //
            // Dropping the table also removes the data stored
            // in that table.
            //
            // Therefore, migration rollback should be handled
            // carefully, especially in production.
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}

