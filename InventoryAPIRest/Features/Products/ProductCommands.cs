using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Services.Interfaces;

namespace InventoryAPIRest.Features.Products
{
    public record CreateProductCommand(CreateProductDto Dto) : ICommand<ProductDto>;
    public record UpdateProductCommand(int Id, UpdateProductDto Dto) : ICommand<bool>;
    public record DeleteProductCommand(int Id) : ICommand<bool>;
    public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductService _service;
        public CreateProductHandler(IProductService service) => _service = service;
        public Task<ProductDto> HandleAsync(CreateProductCommand command) =>
            _service.CreateAsync(command.Dto);
    }

    public class UpdateProductHandler : ICommandHandler<UpdateProductCommand, bool>
    {
        private readonly IProductService _service;
        public UpdateProductHandler(IProductService service) => _service = service;
        public Task<bool> HandleAsync(UpdateProductCommand command) =>
            _service.UpdateAsync(command.Id, command.Dto);
    }

    public class DeleteProductHandler : ICommandHandler<DeleteProductCommand, bool>
    {
        private readonly IProductService _service;
        public DeleteProductHandler(IProductService service) => _service = service;
        public Task<bool> HandleAsync(DeleteProductCommand command) =>
            _service.DeleteAsync(command.Id);
    }
}
