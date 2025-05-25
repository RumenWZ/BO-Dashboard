using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.Business
{
    public class BusinessDto
    {
        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public ICollection<ApplicationUser> Employees { get; set; }

        public ICollection<Models.Service> Services { get; set; }
    }
}
