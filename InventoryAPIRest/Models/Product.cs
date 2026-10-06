using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Models.Enums;

namespace InventoryAPIRest.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<InventoryMovement> Movements { get; set; } = new();

        public InventoryMovement ApplyMovement(MovementType type, int quantity, string? reason)
        {
            if (type == MovementType.Sale && !IsActive)
                throw new InactiveProductException(Id);

            var stockBefore = Stock;
            var stockAfter = type == MovementType.StockIn
                ? stockBefore + quantity
                : stockBefore - quantity;

            if (stockAfter < 0)
                throw new InsufficientStockException(stockBefore, quantity);

            var now = DateTime.UtcNow;
            Stock = stockAfter;
            UpdatedAt = now;

            return new InventoryMovement
            {
                ProductId = Id,
                Type = type,
                Quantity = quantity,
                StockBefore = stockBefore,
                StockAfter = stockAfter,
                UnitPrice = type == MovementType.Sale ? Price : null,
                Reason = reason,
                CreatedAt = now
            };
        }
    }
}
