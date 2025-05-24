using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole,
        IdentityUserLogin<string>, IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUserRole (join entity for Users and Roles)
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            // Business -> Employees (ApplicationUser)
            builder.Entity<Business>()
                .HasMany(b => b.Employees)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            // Business -> Services
            builder.Entity<Business>()
                .HasMany(b => b.Services)
                .WithOne(s => s.Business)
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment -> Service
            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> Employee (ApplicationUser)
            builder.Entity<Appointment>()
                .HasOne(a => a.Employee)
                .WithMany(u => u.AppointmentsAsEmployee)  // Fixed typo here
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> Customer (ApplicationUser)
            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(u => u.AppointmentsAsCustomer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many: ApplicationUser <-> Service via ApplicationUserService
            builder.Entity<ApplicationUserService>()
                .HasKey(au => new { au.UserId, au.ServiceId });

            builder.Entity<ApplicationUserService>()
                .HasOne(au => au.User)
                .WithMany(u => u.ApplicationUserServices)
                .HasForeignKey(au => au.UserId);

            builder.Entity<ApplicationUserService>()
                .HasOne(au => au.Service)
                .WithMany(s => s.ApplicationUserServices)
                .HasForeignKey(au => au.ServiceId);

            // Configure decimal precision for Price property in Service
            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            // Default values for CreatedAt columns
            builder.Entity<ApplicationUser>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("getutcdate()");

            builder.Entity<Service>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("getutcdate()");

            builder.Entity<Appointment>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("getutcdate()");
        }
    }
}
