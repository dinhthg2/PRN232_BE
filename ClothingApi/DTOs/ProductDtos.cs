using System.ComponentModel.DataAnnotations;

namespace ClothingApi.DTOs
{
    public class ProductCreateDto
    {
        [Required, MinLength(2)]
        public string Name { get; set; } = null!;

        [Required, MinLength(4)]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        [Url]
        public string? Image { get; set; }
    }

    public class ProductUpdateDto
    {
        [MinLength(2)]
        public string? Name { get; set; }

        [MinLength(4)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public double? Price { get; set; }

        [Url]
        public string? Image { get; set; }
    }
}
