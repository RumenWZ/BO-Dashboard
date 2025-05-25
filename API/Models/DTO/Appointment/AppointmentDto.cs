using System.Text.Json.Serialization;

namespace API.Models.DTO.Appointment
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }
        public Guid ServiceId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
