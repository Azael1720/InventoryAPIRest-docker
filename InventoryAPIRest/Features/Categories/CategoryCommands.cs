using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Services.Interfaces;

namespace InventoryAPIRest.Features.Categories
{
    public record CreateCategoryCommand(CreateCategoryDto Dto) : ICommand<CategoryDto>;
    public record UpdateCategoryCommand(int Id, UpdateCategoryDto Dto) : ICommand<bool>;
    public record DeleteCategoryCommand(int Id) : ICommand<bool>;

    public class CreateCategoryHandler : ICommandHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryService _service;
        public CreateCategoryHandler(ICategoryService service) => _service = service;
        public Task<CategoryDto> HandleAsync(CreateCategoryCommand command) =>
            _service.CreateAsync(command.Dto);
    }

    public class UpdateCategoryHandler : ICommandHandler<UpdateCategoryCommand, bool>
    {
        private readonly ICategoryService _service;
        public UpdateCategoryHandler(ICategoryService service) => _service = service;
        public Task<bool> HandleAsync(UpdateCategoryCommand command) =>
            _service.UpdateAsync(command.Id, command.Dto);
    }

    public class DeleteCategoryHandler : ICommandHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryService _service;
        public DeleteCategoryHandler(ICategoryService service) => _service = service;
        public Task<bool> HandleAsync(DeleteCategoryCommand command) =>
            _service.DeleteAsync(command.Id);
    }
}
