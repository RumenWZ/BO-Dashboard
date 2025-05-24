using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Business
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Business name cannot exceed 150 characters.")]
        public string Name { get; set; }
        [StringLength(150, ErrorMessage = "Business address cannot exceed 150 characters.")]
        public string? Address { get; set; }
        public ICollection<ApplicationUser> Employees { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}
