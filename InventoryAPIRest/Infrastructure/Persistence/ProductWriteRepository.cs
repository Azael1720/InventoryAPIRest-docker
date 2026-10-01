using InventoryAPIRest.Abstractions;
using InventoryAPIRest.Data;
using InventoryAPIRest.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPIRest.Infrastructure.Persistence
{
    public class ProductWriteRepository : IProductWriteRepository
    {
        private readonly AppDbContext _context;
        public ProductWriteRepository(AppDbContext context) => _context = context;

        public Task<Product?> GetByIdAsync(int id) =>
            _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        public async Task RegisterMovementAsync(Product product, InventoryMovement movement)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var rows = await _context.Products
                .Where(p => p.Id == product.Id && p.Stock == movement.StockBefore)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Stock, movement.StockAfter)
                    .SetProperty(p => p.UpdatedAt, product.UpdatedAt));

            if (rows == 0)
                throw new InvalidOperationException(
                    "El stock cambió mientras se procesaba la operación. Intenta de nuevo más tarde.");

            _context.Entry(product).State = EntityState.Unchanged;
            _context.InventoryMovements.Add(movement);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
    }
}
