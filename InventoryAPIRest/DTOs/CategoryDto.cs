using System.ComponentModel.DataAnnotations;

namespace InventoryAPIRest.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required, StringLength(100)]
        public required string Name { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }
    }

    public class UpdateCategoryDto : CreateCategoryDto { }
}
