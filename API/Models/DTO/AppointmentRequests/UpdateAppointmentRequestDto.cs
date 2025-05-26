using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.AppointmentRequests
{
    public class UpdateAppointmentRequestDto
    {
        [Required]
        public Guid ServiceId { get; set; }
        [Required]
        public DateTime AvailableFrom { get; set; }
        [Required]
        public DateTime AvailableTo { get; set; }
        [StringLength(300)]
        public string? Notes { get; set; }
    }
}
