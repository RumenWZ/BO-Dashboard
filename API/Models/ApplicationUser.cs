using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; } = string.Empty;

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        public bool IsEmployee { get; set; }

        [StringLength(100, ErrorMessage = "Position title cannot exceed 100 characters.")]
        public string? PositionTitle { get; set; }
        public ICollection<ApplicationUserService> ApplicationUserServices { get; set; } = new List<ApplicationUserService>();

        public ICollection<Appointment> AppointmentsAsEmployee { get; set; } = new List<Appointment>();

        public ICollection<Appointment> AppointmentsAsCustomer { get; set; } = new List<Appointment>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
