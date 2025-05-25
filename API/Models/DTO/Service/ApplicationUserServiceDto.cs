namespace API.Models.DTO.Service
{
    public class ApplicationUserServiceDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public Guid ServiceId { get; set; }
    }
}
