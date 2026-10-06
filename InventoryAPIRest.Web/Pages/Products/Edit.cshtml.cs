using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using InventoryAPIRest.Web.Pages;

namespace InventoryAPIRest.Web.Pages.Products
{
    public class EditModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public EditModel(IInventoryApiClient api) => _api = api;

        [BindProperty]
        public ProductForm Form { get; set; } = new();
        public int Id { get; private set; }

        public IReadOnlyList<SelectListItem> Categories { get; private set; } = [];

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            await LoadCategoriesAsync();

            var result = await _api.GetProductAsync(id);
            if (!result.Success)
            {
                SetError(result.Error!.Message);
                return RedirectToPage("Index");
            }

            Form = ProductForm.From(result.Value!);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Id = id;
            await LoadCategoriesAsync();
            if (!ModelState.IsValid) return Page();

            var result = await _api.UpdateProductAsync(id, Form.ToUpdateRequest());
            if (!result.Success)
            {
                AddApiError(result.Error!);
                return Page();
            }

            SetSuccess("Producto actualizado.");
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
