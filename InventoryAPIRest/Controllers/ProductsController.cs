using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPIRest.Controllers
{    
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        public ProductsController(IDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll() =>
        Ok(await _dispatcher.QueryAsync(new GetProductsQuery()));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _dispatcher.QueryAsync(new GetProductByIdQuery(id));
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
        {
            var created = await _dispatcher.SendAsync(new CreateProductCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var updated = await _dispatcher.SendAsync(new UpdateProductCommand(id, dto));
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _dispatcher.SendAsync(new DeleteProductCommand(id));
            return deleted ? NoContent() : NotFound();
        }
    }
}
