using InventoryAPIRest.DTOs;
using InventoryAPIRest.Models;
using InventoryAPIRest.Models.Enums;
using InventoryAPIRest.Repositories.Interfaces;
using InventoryAPIRest.Services.Interfaces;

namespace InventoryAPIRest.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(ToDto);
        }
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product is null ? null : ToDto(product);
        }
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
                throw new KeyNotFoundException($"La categoría con Id {dto.CategoryId} no existe.");

            if (await _productRepository.CodeExistsAsync(dto.Code))
                throw new InvalidOperationException($"Ya existe un producto con el código '{dto.Code}'.");

            var product = new Product
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.Stock > 0)
            {
                product.Movements.Add(new InventoryMovement
                {
                    Type = MovementType.StockIn,
                    Quantity = dto.Stock,
                    StockBefore = 0,
                    StockAfter = dto.Stock,
                    Reason = "Stock inicial",
                    CreatedAt = product.CreatedAt
                });
            }

            var created = await _productRepository.AddAsync(product);
            var withCategory = await _productRepository.GetByIdAsync(created.Id);
            return ToDto(withCategory!);
        }
        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return false;

            if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
                throw new KeyNotFoundException($"La categoría con Id {dto.CategoryId} no existe.");

            if (await _productRepository.CodeExistsAsync(dto.Code, id))
                throw new InvalidOperationException($"Ya existe otro producto con el código '{dto.Code}'.");

            product.Name = dto.Name;
            product.Code = dto.Code;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.IsActive = dto.IsActive;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }
        private static ProductDto ToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
