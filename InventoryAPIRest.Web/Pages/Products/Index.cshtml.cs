using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAPIRest.Web.Pages.Products
{
    public class IndexModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public IndexModel(IInventoryApiClient api) => _api = api;
        public IReadOnlyList<ProductModel> Products { get; private set; } = [];
        public string? LoadError { get; private set; }

        public async Task OnGetAsync()
        {
            var result = await _api.GetProductsAsync();

            if (result.Success)
                Products = result.Value!;
            else
                LoadError = result.Error!.Message;
        }

        public async Task<IActionResult> OnPostDeactivateAsync(int id)
        {
            var result = await _api.DeactivateProductAsync(id);

            if (result.Success)
                SetSuccess("Producto dado de baja.");
            else
                SetError(result.Error!.Message);

            return RedirectToPage();
        }
    }
}
