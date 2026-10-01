using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InventoryAPIRest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class CategoriesController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        public CategoriesController(IDispatcher dispatcher) => _dispatcher = dispatcher;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            try
            {
                return Ok(await _dispatcher.QueryAsync(new GetCategoriesQuery()));
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            try
            {
                var category = await _dispatcher.QueryAsync(new GetCategoryByIdQuery(id));
                return category is null ? NotFound() : Ok(category);
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
        {
            try
            {
                var created = await _dispatcher.SendAsync(new CreateCategoryCommand(dto));
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { message = ex.Message }); 
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            try
            {
                var updated = await _dispatcher.SendAsync(new UpdateCategoryCommand(id, dto));
                return updated ? NoContent() : NotFound();
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _dispatcher.SendAsync(new DeleteCategoryCommand(id));
                return deleted ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}
