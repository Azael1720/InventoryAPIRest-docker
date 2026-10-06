using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAPIRest.Web.Pages.Categories
{
    public class CreateModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public CreateModel(IInventoryApiClient api) => _api = api;

        [BindProperty]
        public CategoryForm Form { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _api.CreateCategoryAsync(Form.ToRequest());
            if (!result.Success)
            {
                AddApiError(result.Error!);
                return Page();
            }

            SetSuccess("Categoría creada.");
            return RedirectToPage("Index");
        }
    }
}
