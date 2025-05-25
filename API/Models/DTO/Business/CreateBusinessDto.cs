using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.Business
{
    public class CreateBusinessDto
    {
        [Required]
        [StringLength(150, ErrorMessage = "Business name cannot exceed 150 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Business address cannot exceed 150 characters.")]
        public string? Address { get; set; }
    }
}
