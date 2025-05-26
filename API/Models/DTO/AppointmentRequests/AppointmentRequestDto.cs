using System.Text.Json.Serialization;

namespace API.Models.DTO.AppointmentRequests
{
    public class AppointmentRequestDto
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public Guid ServiceId { get; set; }
        public Guid? AppointmentId { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string? Notes { get; set; }
    }
}
