using InventoryAPIRest.Data;
using InventoryAPIRest.Models;
using InventoryAPIRest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPIRest.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) => _context = context;
        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null) =>
            await _context.Products.AnyAsync(p => p.Code == code && (excludeId == null || p.Id != excludeId));
        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }   
        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
