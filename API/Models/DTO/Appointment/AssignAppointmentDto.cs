namespace API.Models.DTO.Appointment
{
    public class AssignAppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
    }
}
