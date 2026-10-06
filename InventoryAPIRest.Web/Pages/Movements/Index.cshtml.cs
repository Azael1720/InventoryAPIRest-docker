using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAPIRest.Web.Pages.Movements
{
    public class IndexModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public IndexModel(IInventoryApiClient api) => _api = api;

        public int? ProductId { get; private set; }
        public IReadOnlyList<ProductModel> Products { get; private set; } = [];
        public IReadOnlyList<MovementModel> Movements { get; private set; } = [];
        public string? LoadError { get; private set; }

        public async Task OnGetAsync(int? productId)
        {
            ProductId = productId;

            var products = await _api.GetProductsAsync();
            if (products.Success)
                Products = products.Value!;

            var movements = productId is null
                ? await _api.GetMovementsAsync()
                : await _api.GetProductMovementsAsync(productId.Value);

            if (movements.Success)
                Movements = movements.Value!;
            else
                LoadError = movements.Error!.Message;
        }
    }
}
