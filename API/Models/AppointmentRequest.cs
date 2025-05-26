using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AppointmentRequest
    {
        public Guid Id { get; set; }
        [Required]
        public string CustomerId { get; set; } = null!;
        public virtual ApplicationUser Customer { get; set; } = null!;
        [Required]
        public Guid ServiceId { get; set; }
        public virtual Service Service { get; set; } = null!;
        public Guid? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        [StringLength(300)]
        public string? Notes { get; set; }
    }
}

public enum RequestStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled
}