using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Categories;
using InventoryAPIRest.Features.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPIRest.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]    
    public class CategoriesController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        public CategoriesController(IDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll() =>
        Ok(await _dispatcher.QueryAsync(new GetCategoriesQuery()));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _dispatcher.QueryAsync(new GetCategoryByIdQuery(id));
            return category is null ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
        {
            var created = await _dispatcher.SendAsync(new CreateCategoryCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            var updated = await _dispatcher.SendAsync(new UpdateCategoryCommand(id, dto));
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _dispatcher.SendAsync(new DeleteCategoryCommand(id));
            return deleted ? NoContent() : NotFound();
        }
    }
}
