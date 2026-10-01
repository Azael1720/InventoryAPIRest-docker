using InventoryAPIRest.DTOs;
using InventoryAPIRest.Models;

namespace InventoryAPIRest.Features.Inventory
{
    internal static class MovementMapping
    {
        public static InventoryMovementDto ToDto(this InventoryMovement movement, Product product) => new()
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = product.Name,
            ProductCode = product.Code,
            Type = movement.Type.ToString(),
            Quantity = movement.Quantity,
            StockBefore = movement.StockBefore,
            StockAfter = movement.StockAfter,
            UnitPrice = movement.UnitPrice,
            Reason = movement.Reason,
            CreatedAt = movement.CreatedAt
        };
    }
}
