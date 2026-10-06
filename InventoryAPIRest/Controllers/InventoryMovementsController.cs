using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPIRest.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]    
    public class InventoryMovementsController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public InventoryMovementsController(IDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost("products/{productId:int}/stock-in")]
        public async Task<ActionResult<InventoryMovementDto>> AddStock(int productId, StockOperationDto dto) =>
        Ok(await _dispatcher.SendAsync(new AddStockCommand(productId, dto)));

        [HttpPost("products/{productId:int}/stock-out")]
        public async Task<ActionResult<InventoryMovementDto>> RemoveStock(int productId, StockOperationDto dto) =>
            Ok(await _dispatcher.SendAsync(new RemoveStockCommand(productId, dto)));

        [HttpPost("products/{productId:int}/sales")]
        public async Task<ActionResult<InventoryMovementDto>> Sell(int productId, StockOperationDto dto) =>
            Ok(await _dispatcher.SendAsync(new SellProductCommand(productId, dto)));

        [HttpGet("products/{productId:int}/movements")]
        public async Task<ActionResult<IEnumerable<InventoryMovementDto>>> GetByProduct(int productId) =>
            Ok(await _dispatcher.QueryAsync(new GetMovementsByProductQuery(productId)));

        [HttpGet("inventory-movements")]
        public async Task<ActionResult<IEnumerable<InventoryMovementDto>>> GetAll() =>
            Ok(await _dispatcher.QueryAsync(new GetMovementsQuery()));
    }
}
