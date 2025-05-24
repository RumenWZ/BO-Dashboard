using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Business
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Business name cannot exceed 150 characters.")]
        [MinLength(1, ErrorMessage = "Business name cannot be empty.")]
        public string Name { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Business address cannot exceed 150 characters.")]
        [MinLength(1, ErrorMessage = "Business address cannot be empty.")]
        public string? Address { get; set; }

        public ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
