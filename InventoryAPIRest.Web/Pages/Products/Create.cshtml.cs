using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using InventoryAPIRest.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryAPIRest.Web.Pages.Products
{
    public class CreateModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public CreateModel(IInventoryApiClient api) => _api = api;

        [BindProperty]
        public ProductForm Form { get; set; } = new();
        public IReadOnlyList<SelectListItem> Categories { get; private set; } = [];

        public Task OnGetAsync() => LoadCategoriesAsync();
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategoriesAsync();
            if (!ModelState.IsValid) return Page();

            var result = await _api.CreateProductAsync(Form.ToCreateRequest());
            if (!result.Success)
            {
                AddApiError(result.Error!);
                return Page();
            }

            SetSuccess("Producto creado.");
            return RedirectToPage("Index");
        }

        private async Task LoadCategoriesAsync()
        {
            var result = await _api.GetCategoriesAsync();
            if (!result.Success)
            {
                AddApiError(result.Error!);
                return;
            }

            Categories = result.Value!
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
        }
    }
}
