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
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Models.Service> Services { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<ApplicationUserService> ApplicationUserServices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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

            builder.Entity<Business>()
                .HasMany(b => b.Employees)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Business>()
                .HasMany(b => b.Services)
                .WithOne(s => s.Business)
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Employee)
                .WithMany(u => u.AppointmentsAsEmployee)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(u => u.AppointmentsAsCustomer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

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

            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            // Default values
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
