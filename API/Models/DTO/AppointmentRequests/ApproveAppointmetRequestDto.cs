using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.AppointmentRequests
{
    public class ApproveAppointmetRequestDto
    {
        [Required]
        public string EmployeeId { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
