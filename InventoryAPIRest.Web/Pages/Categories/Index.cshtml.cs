using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using InventoryAPIRest.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAPIRest.Web.Pages.Categories
{
    public class IndexModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public IndexModel(IInventoryApiClient api) => _api = api;

        public IReadOnlyList<CategoryModel> Categories { get; private set; } = [];
        public string? LoadError { get; private set; }

        public async Task OnGetAsync()
        {
            var result = await _api.GetCategoriesAsync();

            if (result.Success)
                Categories = result.Value!;
            else
                LoadError = result.Error!.Message;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _api.DeleteCategoryAsync(id);

            if (result.Success)
                SetSuccess("Categoría eliminada.");
            else
                SetError(result.Error!.Message);

            return RedirectToPage();
        }
    }
}
