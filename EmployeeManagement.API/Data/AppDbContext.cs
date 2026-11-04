
using EmployeeManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Data
{
    // Represents the application's database context.
    public class AppDbContext : DbContext
    {
        // Constructor receives database configuration through DI.
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Database tables.
        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<User> Users => Set<User>();

        public DbSet<Department> Departments => Set<Department>();

        // Configure entity-to-database mappings.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // EMPLOYEE CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Salary)
                    .HasPrecision(18, 2);
            });

            // ==========================================
            // USER CONFIGURATION
            // ==========================================

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Password)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(20);

                // Username must be unique.
                entity.HasIndex(u => u.Username)
                    .IsUnique();
            });

            // ==========================================
            // DEPARTMENT CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(d => d.Description)
                    .HasMaxLength(500);

                // Department names must be unique.
                entity.HasIndex(d => d.Name)
                    .IsUnique();
            });
        }
    }
}
