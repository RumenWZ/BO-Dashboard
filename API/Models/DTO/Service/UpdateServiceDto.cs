using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO.Service
{
    public class UpdateServiceDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and €10,000.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        [DataType(DataType.Duration)]
        public int DurationInMinutes { get; set; }

        [Required(ErrorMessage = "Business ID is required.")]
        public Guid BusinessId { get; set; }
    }
}
