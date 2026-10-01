using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Services.Interfaces;

namespace InventoryAPIRest.Features.Categories
{
    public record GetCategoriesQuery : IQuery<IEnumerable<CategoryDto>>;
    public record GetCategoryByIdQuery(int Id) : IQuery<CategoryDto?>;
    public class GetCategoriesHandler : IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly ICategoryService _service;
        public GetCategoriesHandler(ICategoryService service) => _service = service;
        public Task<IEnumerable<CategoryDto>> HandleAsync(GetCategoriesQuery query) =>
            _service.GetAllAsync();
    }

    public class GetCategoryByIdHandler : IQueryHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryService _service;
        public GetCategoryByIdHandler(ICategoryService service) => _service = service;
        public Task<CategoryDto?> HandleAsync(GetCategoryByIdQuery query) =>
            _service.GetByIdAsync(query.Id);
    }
}
