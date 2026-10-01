using InventoryAPIRest.Models;


namespace InventoryAPIRest.Abstractions
{
    public interface IProductWriteRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task RegisterMovementAsync(Product product, InventoryMovement movement);
    }
}
