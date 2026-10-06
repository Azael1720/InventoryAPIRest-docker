using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAPIRest.Web.Pages.Categories
{
    public class EditModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public EditModel(IInventoryApiClient api) => _api = api;

        [BindProperty]
        public CategoryForm Form { get; set; } = new();
        public int Id { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;

            var result = await _api.GetCategoryAsync(id);
            if (!result.Success)
            {
                SetError(result.Error!.Message);
                return RedirectToPage("Index");
            }

            Form = CategoryForm.From(result.Value!);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            Id = id;
            if (!ModelState.IsValid) return Page();

            var result = await _api.UpdateCategoryAsync(id, Form.ToRequest());
            if (!result.Success)
            {
                AddApiError(result.Error!);
                return Page();
            }

            SetSuccess("Categoría actualizada.");
            return RedirectToPage("Index");
        }
    }
}
