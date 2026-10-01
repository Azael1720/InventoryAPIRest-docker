using InventoryAPIRest.Data;
using InventoryAPIRest.Models;
using InventoryAPIRest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPIRest.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) => _context = context;
        public async Task<IEnumerable<Category>> GetAllAsync() =>
            await _context.Categories.AsNoTracking().ToListAsync();
        public async Task<Category?> GetByIdAsync(int id) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        public async Task<bool> ExistsAsync(int id) =>
            await _context.Categories.AnyAsync(c => c.Id == id);
        public async Task<bool> HasProductsAsync(int id) =>
            await _context.Products.AnyAsync(p => p.CategoryId == id);
        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
