namespace API.Models.DTO.Service
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
        public Guid BusinessId { get; set; }
        public ICollection<ApplicationUserServiceDto> ApplicationUserServices { get; set; } = new List<ApplicationUserServiceDto>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
