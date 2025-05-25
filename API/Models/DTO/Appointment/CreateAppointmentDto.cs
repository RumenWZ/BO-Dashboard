using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.Appointment
{
    public class CreateAppointmentDto
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public string CustomerId { get; set; }
        [StringLength(300, ErrorMessage = "Note cannot exceed 300 characters.")]
        public string? Notes { get; set; }
    }
}
