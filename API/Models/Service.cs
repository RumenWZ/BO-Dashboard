using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Service
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters.")]
        [MinLength(1, ErrorMessage = "Service name cannot be empty.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500, ErrorMessage = "Service description cannot exceed 500 characters.")]
        [MinLength(1, ErrorMessage = "Service description cannot be empty.")]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be zero or positive.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int DurationInMinutes { get; set; }

        public Guid BusinessId { get; set; }
        public virtual Business? Business { get; set; }

        public ICollection<ApplicationUserService> ApplicationUserServices { get; set; } = new List<ApplicationUserService>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
