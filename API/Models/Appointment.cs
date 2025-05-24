using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid ServiceId { get; set; }
        [Required]
        public virtual Service Service { get; set; } = null!;

        public string EmployeeId { get; set; } = null!;
        [Required]
        public virtual ApplicationUser Employee { get; set; } = null!;

        public string CustomerId { get; set; } = null!;
        [Required]
        public virtual ApplicationUser Customer { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Cancelled,
        Completed,
        Rescheduled
    }
}
