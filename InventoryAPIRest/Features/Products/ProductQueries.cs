using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Services.Interfaces;

namespace InventoryAPIRest.Features.Products
{
    public record GetProductsQuery : IQuery<IEnumerable<ProductDto>>;
    public record GetProductByIdQuery(int Id) : IQuery<ProductDto?>;

    public class GetProductsHandler : IQueryHandler<GetProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductService _service;
        public GetProductsHandler(IProductService service) => _service = service;
        public Task<IEnumerable<ProductDto>> HandleAsync(GetProductsQuery query) =>
            _service.GetAllAsync();
    }

    public class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductService _service;
        public GetProductByIdHandler(IProductService service) => _service = service;
        public Task<ProductDto?> HandleAsync(GetProductByIdQuery query) =>
            _service.GetByIdAsync(query.Id);
    }
}
