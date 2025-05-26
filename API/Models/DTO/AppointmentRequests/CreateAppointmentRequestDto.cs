using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.AppointmentRequests
{
    public class CreateAppointmentRequestDto
    {
        [Required]
        public string CustomerId { get; set; } = null!;
        [Required]
        public Guid ServiceId { get; set; }

        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }
    }
}
