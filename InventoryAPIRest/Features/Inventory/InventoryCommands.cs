using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Models.Enums;
using InventoryAPIRest.Services.Interfaces;

namespace InventoryAPIRest.Features.Inventory
{
    public record AddStockCommand(int ProductId, StockOperationDto Dto) : ICommand<InventoryMovementDto>;
    public record RemoveStockCommand(int ProductId, StockOperationDto Dto) : ICommand<InventoryMovementDto>;
    public record SellProductCommand(int ProductId, StockOperationDto Dto) : ICommand<InventoryMovementDto>;

    public class AddStockHandler
        : StockMovementHandlerBase, ICommandHandler<AddStockCommand, InventoryMovementDto>
    {
        public AddStockHandler(IProductWriteRepository products) : base(products) { }

        public Task<InventoryMovementDto> HandleAsync(AddStockCommand command) =>
            RegisterAsync(command.ProductId, MovementType.StockIn, command.Dto);
    }

    public class RemoveStockHandler
        : StockMovementHandlerBase, ICommandHandler<RemoveStockCommand, InventoryMovementDto>
    {
        public RemoveStockHandler(IProductWriteRepository products) : base(products) { }

        public Task<InventoryMovementDto> HandleAsync(RemoveStockCommand command) =>
            RegisterAsync(command.ProductId, MovementType.StockOut, command.Dto);
    }

    public class SellProductHandler
        : StockMovementHandlerBase, ICommandHandler<SellProductCommand, InventoryMovementDto>
    {
        public SellProductHandler(IProductWriteRepository products) : base(products) { }

        public Task<InventoryMovementDto> HandleAsync(SellProductCommand command) =>
            RegisterAsync(command.ProductId, MovementType.Sale, command.Dto);
    }
}
