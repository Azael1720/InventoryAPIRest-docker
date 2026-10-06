using InventoryAPIRest.Web.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAPIRest.Web.Pages
{
    public class AppPageModel : PageModel
    {
        protected void AddApiError(ApiError error)
        {
            if (error.Details is { Count: > 0 })
            {
                foreach (var detail in error.Details)
                    ModelState.AddModelError(string.Empty, detail);
            }
            else
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
        }
        protected void SetSuccess(string message) => TempData["Success"] = message;
        protected void SetError(string message) => TempData["Error"] = message;
    }
}
