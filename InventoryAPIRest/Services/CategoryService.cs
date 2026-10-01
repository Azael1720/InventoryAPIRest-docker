using InventoryAPIRest.DTOs;
using InventoryAPIRest.Models;
using InventoryAPIRest.Repositories.Interfaces;
using InventoryAPIRest.Services.Interfaces;

namespace InventoryAPIRest.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        public CategoryService(ICategoryRepository repository) => _repository = repository;
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(ToDto);
        }
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category is null ? null : ToDto(category);
        }
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var created = await _repository.AddAsync(category);
            return ToDto(created);
        }
        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category is null) return false;

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _repository.UpdateAsync(category);
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category is null) return false;

            if (await _repository.HasProductsAsync(id))
                throw new InvalidOperationException("No se puede eliminar una categoría que tiene productos asociados.");

            await _repository.DeleteAsync(category);
            return true;
        }
        private static CategoryDto ToDto(Category c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        };
    }
}
