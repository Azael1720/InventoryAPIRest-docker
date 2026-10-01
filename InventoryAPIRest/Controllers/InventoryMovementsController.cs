using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPIRest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class InventoryMovementsController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public InventoryMovementsController(IDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpPost("products/{productId:int}/stock-in")]
        public Task<ActionResult<InventoryMovementDto>> AddStock(int productId, StockOperationDto dto) =>
            Execute(new AddStockCommand(productId, dto));

        [HttpPost("products/{productId:int}/stock-out")]
        public Task<ActionResult<InventoryMovementDto>> RemoveStock(int productId, StockOperationDto dto) =>
            Execute(new RemoveStockCommand(productId, dto));

        [HttpPost("products/{productId:int}/sales")]
        public Task<ActionResult<InventoryMovementDto>> Sell(int productId, StockOperationDto dto) =>
            Execute(new SellProductCommand(productId, dto));

        [HttpGet("products/{productId:int}/movements")]
        public async Task<ActionResult<IEnumerable<InventoryMovementDto>>> GetByProduct(int productId)
        {
            try
            {
                return Ok(await _dispatcher.QueryAsync(new GetMovementsByProductQuery(productId)));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpGet("inventory-movements")]
        public async Task<ActionResult<IEnumerable<InventoryMovementDto>>> GetAll()
        {
            try
            {
                return Ok(await _dispatcher.QueryAsync(new GetMovementsQuery()));
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }
        private async Task<ActionResult<InventoryMovementDto>> Execute(ICommand<InventoryMovementDto> command)
        {
            try
            {
                return Ok(await _dispatcher.SendAsync(command));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}
