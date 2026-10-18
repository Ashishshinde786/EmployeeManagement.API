
// ============================================================
// NAMESPACE
// ============================================================

// This namespace contains the model/entity classes used by
// the Employee Management API.
//
// Models represent the application's main data objects.
//
// In this project, Employee represents an employee record
// that will eventually be stored in the Employees database
// table.
namespace EmployeeManagement.API.Models
{
    // ========================================================
    // EMPLOYEE MODEL / ENTITY
    // ========================================================

    // Employee is an Entity class.
    //
    // This class represents an employee in our application.
    //
    // Entity Framework Core uses this class to map our C#
    // object to the Employees table in SQL Server.
    //
    // The general relationship is:
    //
    // Employee class
    //       ↓
    // Entity Framework Core
    //       ↓
    // Employees table
    //
    public class Employee
    {
        // ====================================================
        // EMPLOYEE ID
        // ====================================================

        // Id uniquely identifies each employee.
        //
        // It is an integer value.
        //
        // Example:
        //
        // Employee 1 → Id = 1
        // Employee 2 → Id = 2
        // Employee 3 → Id = 3
        //
        // In the database, EF Core configures this property
        // as the primary key.
        //
        // Because Id is an integer primary key, SQL Server
        // can automatically generate its value using an
        // Identity column.
        public int Id { get; set; }


        // ====================================================
        // EMPLOYEE NAME
        // ====================================================

        // Stores the employee's name.
        //
        // Example:
        //
        // "Ashish Shinde"
        //
        // string.Empty initializes the property with an empty
        // string instead of null.
        //
        // Important:
        //
        // string.Empty itself does not perform validation.
        // Validation rules are normally defined in DTOs such
        // as CreateEmployeeRequest and UpdateEmployeeRequest.
        public string Name { get; set; } = string.Empty;


        // ====================================================
        // EMPLOYEE EMAIL
        // ====================================================

        // Stores the employee's email address.
        //
        // Example:
        //
        // "ashish@example.com"
        //
        // The API DTO uses [EmailAddress] to validate the
        // incoming email format.
        //
        // The entity itself represents the data that is
        // persisted in the database.
        public string Email { get; set; } = string.Empty;


        // ====================================================
        // EMPLOYEE DEPARTMENT
        // ====================================================

        // Stores the department in which the employee works.
        //
        // Examples:
        //
        // "IT"
        // "HR"
        // "Finance"
        // "Operations"
        //
        // The value is stored as a string in the database.
        public string Department { get; set; } = string.Empty;


        // ====================================================
        // EMPLOYEE SALARY
        // ====================================================

        // Stores the employee's salary.
        //
        // decimal is used instead of double/float because
        // salary is a monetary/financial value and decimal
        // provides appropriate decimal precision.
        //
        // In your AppDbContext, Salary is configured as:
        //
        // .HasPrecision(18, 2)
        //
        // Therefore SQL Server stores it as:
        //
        // decimal(18,2)
        //
        // Example:
        //
        // 75000.00
        // 85000.50
        public decimal Salary { get; set; }
    }
}


/*
============================================================
CLASS EXPLANATION: Employee
============================================================

Employee is a MODEL / ENTITY class in the application.

It represents an employee's data in the application and is
mapped by Entity Framework Core to the Employees table in
SQL Server.


------------------------------------------------------------
WHY DO WE NEED THE EMPLOYEE CLASS?
------------------------------------------------------------

We need this class to represent employee data as a C# object.

Instead of working directly with database rows everywhere,
we can work with an Employee object.

For example:

    Employee employee = new Employee();

    employee.Name = "Ashish";
    employee.Email = "ashish@example.com";
    employee.Department = "IT";
    employee.Salary = 85000;


Entity Framework Core then maps this object to the database.


------------------------------------------------------------
EMPLOYEE CLASS → DATABASE TABLE
------------------------------------------------------------

The relationship is:

    Employee
       ↓
    Entity Framework Core
       ↓
    Employees table


The properties become database columns:

    Employee.Id
        ↓
    Employees.Id

    Employee.Name
        ↓
    Employees.Name

    Employee.Email
        ↓
    Employees.Email

    Employee.Department
        ↓
    Employees.Department

    Employee.Salary
        ↓
    Employees.Salary


------------------------------------------------------------
ID PROPERTY
------------------------------------------------------------

    public int Id { get; set; }

Id uniquely identifies an employee.

In your EF Core configuration, Id becomes the primary key.

SQL Server also generates the value automatically because
it is configured as an Identity column.

Example:

    Employee 1 → Id = 1
    Employee 2 → Id = 2
    Employee 3 → Id = 3


------------------------------------------------------------
NAME PROPERTY
------------------------------------------------------------

    public string Name { get; set; }

Stores the employee's name.

Example:

    "Ashish Shinde"


The validation rules for API input are defined separately
in DTOs such as:

    CreateEmployeeRequest
    UpdateEmployeeRequest


------------------------------------------------------------
EMAIL PROPERTY
------------------------------------------------------------

    public string Email { get; set; }

Stores the employee's email address.

Example:

    "ashish@example.com"


The DTO uses [EmailAddress] to validate the format of the
incoming value.


------------------------------------------------------------
DEPARTMENT PROPERTY
------------------------------------------------------------

    public string Department { get; set; }

Stores the employee's department.

Examples:

    IT
    HR
    Finance
    Operations


------------------------------------------------------------
SALARY PROPERTY
------------------------------------------------------------

    public decimal Salary { get; set; }

Stores the employee's salary.

decimal is used because salary is a monetary value.

Your AppDbContext configures:

    .HasPrecision(18, 2)

So SQL Server uses:

    decimal(18,2)


For example:

    85000.50


------------------------------------------------------------
WHY USE AN ENTITY CLASS?
------------------------------------------------------------

The Employee entity is mainly responsible for representing
persistent application data.

It works with Entity Framework Core.

Your architecture is approximately:

    Controller
        ↓
    Service
        ↓
    Repository
        ↓
    AppDbContext
        ↓
    Employee Entity
        ↓
    SQL Server


------------------------------------------------------------
ENTITY VS DTO
------------------------------------------------------------

Employee is the DATABASE / PERSISTENCE MODEL.

For example:

    Employee
        ↓
    Database


CreateEmployeeRequest and UpdateEmployeeRequest are DTOs.

For example:

    Client
        ↓
    CreateEmployeeRequest
        ↓
    Service
        ↓
    Employee
        ↓
    Database


EmployeeResponse is an OUTPUT DTO.

For example:

    Database
        ↓
    Employee
        ↓
    EmployeeResponse
        ↓
    Client


This separation prevents the database entity from becoming
directly tied to the API request and response contracts.


------------------------------------------------------------
COMPLETE FLOW
------------------------------------------------------------

When creating an employee:

    Client
       ↓
    CreateEmployeeRequest
       ↓
    EmployeeController
       ↓
    EmployeeService
       ↓
    Employee entity
       ↓
    AppDbContext
       ↓
    Entity Framework Core
       ↓
    SQL Server
       ↓
    Employees table


When reading an employee:

    SQL Server
       ↓
    Employees table
       ↓
    Entity Framework Core
       ↓
    Employee entity
       ↓
    EmployeeResponse
       ↓
    Client


------------------------------------------------------------
IN SIMPLE WORDS
------------------------------------------------------------

Employee is the main C# entity that represents an employee
record in the Employee Management application.

Entity Framework Core uses this class to map employee objects
to the Employees table in SQL Server.

It contains:

    Id
    Name
    Email
    Department
    Salary

The class focuses on representing the employee data, while
DTOs are used to control what data enters and leaves the API.

============================================================
END OF Employee CLASS EXPLANATION
============================================================
*/

