using InventoryAPIRest.Web.Infrastructure.Api;
using InventoryAPIRest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAPIRest.Web.Pages.Movements
{
    public class RegisterModel : AppPageModel
    {
        private readonly IInventoryApiClient _api;
        public RegisterModel(IInventoryApiClient api) => _api = api;
        public static readonly IReadOnlyList<(string Value, string Label)> MovementTypes =
        [
            ("StockIn", "Entrada de stock"),
            ("StockOut", "Salida de stock"),
            ("Sale", "Venta")
        ];

        [BindProperty]
        public MovementForm Form { get; set; } = new();
        public IReadOnlyList<ProductModel> Products { get; private set; } = [];

        public async Task OnGetAsync(int? productId)
        {
            Form.ProductId = productId;
            await LoadProductsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadProductsAsync();
            if (!ModelState.IsValid) return Page();

            var reason = string.IsNullOrWhiteSpace(Form.Reason) ? null : Form.Reason.Trim();
            var request = new StockRequest(Form.Quantity!.Value, reason);

            var result = await _api.RegisterMovementAsync(Form.ProductId!.Value, Form.Type!, request);
            if (!result.Success)
            {
                AddApiError(result.Error!);
                return Page();
            }

            SetSuccess($"Movimiento registrado. Stock: {result.Value!.StockBefore} → {result.Value.StockAfter}.");
            return RedirectToPage("Index", new { productId = Form.ProductId });
        }

        private async Task LoadProductsAsync()
        {
            var result = await _api.GetProductsAsync();

            if (result.Success)
                Products = result.Value!;
            else
                AddApiError(result.Error!);
        }
    }
}
