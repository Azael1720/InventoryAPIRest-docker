using System.ComponentModel.DataAnnotations;

namespace InventoryAPIRest.Web.Models
{
    public class CategoryForm
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        public string? Description { get; set; }

        public static CategoryForm From(CategoryModel category) =>
            new() { Name = category.Name, Description = category.Description };

        public CategoryRequest ToRequest() => new(Name.Trim(), NullIfBlank(Description));

        private static string? NullIfBlank(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public class ProductForm
    {
        [Display(Name = "Código")]
        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        public string? Description { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 100000000, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal? Price { get; set; }

        [Display(Name = "Stock inicial")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "Selecciona una categoría.")]
        public int? CategoryId { get; set; }

        [Display(Name = "Producto activo")]
        public bool IsActive { get; set; } = true;

        public static ProductForm From(ProductModel product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive
        };

        public CreateProductRequest ToCreateRequest() =>
            new(Name.Trim(), Code.Trim(), NullIfBlank(Description), Price!.Value, Stock, CategoryId!.Value);

        public UpdateProductRequest ToUpdateRequest() =>
            new(Name.Trim(), Code.Trim(), NullIfBlank(Description), Price!.Value, CategoryId!.Value, IsActive);

        private static string? NullIfBlank(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public class MovementForm
    {
        [Display(Name = "Producto")]
        [Required(ErrorMessage = "Selecciona un producto.")]
        public int? ProductId { get; set; }

        [Display(Name = "Tipo de movimiento")]
        [Required(ErrorMessage = "Selecciona el tipo de movimiento.")]
        [RegularExpression("^(StockIn|StockOut|Sale)$", ErrorMessage = "Tipo de movimiento no válido.")]
        public string? Type { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int? Quantity { get; set; }

        [Display(Name = "Motivo")]
        [StringLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        public string? Reason { get; set; }
    }
}
