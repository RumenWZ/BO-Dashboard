namespace API.Models.DTO.Appointment
{
    public class UpdateAppointmentDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid ServiceId { get; set; }
        public string EmployeeId { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public string? Notes { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    }
}
