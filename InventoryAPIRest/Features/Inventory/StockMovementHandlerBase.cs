using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Models.Enums;

namespace InventoryAPIRest.Features.Inventory
{
    public abstract class StockMovementHandlerBase
    {
        private readonly IProductWriteRepository _products;
        protected StockMovementHandlerBase(IProductWriteRepository products) => _products = products;

        protected async Task<InventoryMovementDto> RegisterAsync(
            int productId, MovementType type, StockOperationDto dto)
        {
            var product = await _products.GetByIdAsync(productId)
                ?? throw new ProductNotFoundException(productId);
            var movement = product.ApplyMovement(type, dto.Quantity, dto.Reason);

            await _products.RegisterMovementAsync(product, movement);

            return movement.ToDto(product);
        }
    }
}
