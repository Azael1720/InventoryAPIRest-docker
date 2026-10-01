using System.ComponentModel.DataAnnotations;

namespace InventoryAPIRest.DTOs
{
    public class InventoryMovementDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class StockOperationDto
    {
        [Range(1, 1000000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Quantity { get; set; }

        [StringLength(300)]
        public string? Reason { get; set; }
    }
}
