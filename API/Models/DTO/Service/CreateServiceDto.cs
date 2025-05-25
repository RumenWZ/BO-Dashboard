using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.Service
{
    public class CreateServiceDto
    {
        [Required]
        [StringLength(100)]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [MinLength(1)]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue)]
        public int DurationInMinutes { get; set; }

        [Required]
        public Guid BusinessId { get; set; }

    }
}
