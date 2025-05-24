namespace API.Models
{
    public class ApplicationUserService
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
