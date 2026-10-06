using System.Net.Http.Json;
using InventoryAPIRest.Web.Infrastructure.Auth;
using InventoryAPIRest.Web.Models;
using System.Text.Json;

namespace InventoryAPIRest.Web.Infrastructure.Api
{
    public interface IInventoryApiClient
    {
        Task<ApiResult<List<CategoryModel>>> GetCategoriesAsync();
        Task<ApiResult<CategoryModel>> GetCategoryAsync(int id);
        Task<ApiResult> CreateCategoryAsync(CategoryRequest request);
        Task<ApiResult> UpdateCategoryAsync(int id, CategoryRequest request);
        Task<ApiResult> DeleteCategoryAsync(int id);

        Task<ApiResult<List<ProductModel>>> GetProductsAsync();
        Task<ApiResult<ProductModel>> GetProductAsync(int id);
        Task<ApiResult> CreateProductAsync(CreateProductRequest request);
        Task<ApiResult> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<ApiResult> DeactivateProductAsync(int id);

        Task<ApiResult<List<MovementModel>>> GetMovementsAsync();
        Task<ApiResult<List<MovementModel>>> GetProductMovementsAsync(int productId);
        Task<ApiResult<MovementModel>> RegisterMovementAsync(int productId, string type, StockRequest request);
    }
    public class InventoryApiClient : IInventoryApiClient
    {
        private const string CategoriesRoute = "api/Categories";
        private const string ProductsRoute = "api/Products";
        private const string MovementsRoute = "api/InventoryMovements";

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        private readonly HttpClient _http;
        private readonly ILogger<InventoryApiClient> _logger;

        public InventoryApiClient(HttpClient http, ILogger<InventoryApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public Task<ApiResult<List<CategoryModel>>> GetCategoriesAsync() =>
            GetAsync<List<CategoryModel>>(CategoriesRoute);

        public Task<ApiResult<CategoryModel>> GetCategoryAsync(int id) =>
            GetAsync<CategoryModel>($"{CategoriesRoute}/{id}");
        public Task<ApiResult> CreateCategoryAsync(CategoryRequest request) =>
            SendWithoutResultAsync(HttpMethod.Post, CategoriesRoute, request);
        public Task<ApiResult> UpdateCategoryAsync(int id, CategoryRequest request) =>
            SendWithoutResultAsync(HttpMethod.Put, $"{CategoriesRoute}/{id}", request);
        public Task<ApiResult> DeleteCategoryAsync(int id) =>
            SendWithoutResultAsync(HttpMethod.Delete, $"{CategoriesRoute}/{id}");

        public Task<ApiResult<List<ProductModel>>> GetProductsAsync() =>
            GetAsync<List<ProductModel>>(ProductsRoute);
        public Task<ApiResult<ProductModel>> GetProductAsync(int id) =>
            GetAsync<ProductModel>($"{ProductsRoute}/{id}");
        public Task<ApiResult> CreateProductAsync(CreateProductRequest request) =>
            SendWithoutResultAsync(HttpMethod.Post, ProductsRoute, request);
        public Task<ApiResult> UpdateProductAsync(int id, UpdateProductRequest request) =>
            SendWithoutResultAsync(HttpMethod.Put, $"{ProductsRoute}/{id}", request);
        public Task<ApiResult> DeactivateProductAsync(int id) =>
            SendWithoutResultAsync(HttpMethod.Delete, $"{ProductsRoute}/{id}");

        public Task<ApiResult<List<MovementModel>>> GetMovementsAsync() =>
            GetAsync<List<MovementModel>>($"{MovementsRoute}/inventory-movements");
        public Task<ApiResult<List<MovementModel>>> GetProductMovementsAsync(int productId) =>
            GetAsync<List<MovementModel>>($"{MovementsRoute}/products/{productId}/movements");
        public Task<ApiResult<MovementModel>> RegisterMovementAsync(int productId, string type, StockRequest request)
        {
            var action = type switch
            {
                "StockIn" => "stock-in",
                "StockOut" => "stock-out",
                "Sale" => "sales",
                _ => throw new ArgumentException($"Tipo de movimiento no válido: {type}", nameof(type))
            };

            return SendAsync<MovementModel>(
                HttpMethod.Post,
                $"{MovementsRoute}/products/{productId}/{action}",
                request,
                ReadJsonAsync<MovementModel>);
        }

        private Task<ApiResult<T>> GetAsync<T>(string url) =>
            SendAsync<T>(HttpMethod.Get, url, null, ReadJsonAsync<T>);

        private async Task<ApiResult> SendWithoutResultAsync(HttpMethod method, string url, object? body = null) =>
            await SendAsync<bool>(method, url, body, _ => Task.FromResult(true));

        private async Task<ApiResult<T>> SendAsync<T>(
            HttpMethod method, string url, object? body, Func<HttpResponseMessage, Task<T>> readSuccess)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);
                if (body is not null)
                    request.Content = JsonContent.Create(body, body.GetType(), options: JsonOptions);

                using var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorAsync(response);
                    _logger.LogWarning("La API respondió {Status} a {Method} {Url}", error.Status, method, url);
                    return ApiResult<T>.Fail(error);
                }

                return ApiResult<T>.Ok(await readSuccess(response));
            }
            catch (AuthenticationFailedException ex)
            {
                _logger.LogError(ex, "No se pudo obtener el token de acceso");
                return ApiResult<T>.Fail(new ApiError(401,
                    "No fue posible autenticarse con Keycloak. Verifica el cliente configurado y que Keycloak esté en ejecución."));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "No se pudo conectar con la API");
                return ApiResult<T>.Fail(new ApiError(503,
                    "No se pudo conectar con la API. Verifica que esté en ejecución."));
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "La API no respondió a tiempo");
                return ApiResult<T>.Fail(new ApiError(504,
                    "La API tardó demasiado en responder. Intenta de nuevo."));
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException)
            {
                _logger.LogError(ex, "Respuesta de la API con formato inesperado");
                return ApiResult<T>.Fail(new ApiError(502,
                    "La respuesta de la API no tiene el formato esperado."));
            }
        }

        private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response) =>
            await response.Content.ReadFromJsonAsync<T>(JsonOptions)
            ?? throw new JsonException("La API devolvió una respuesta vacía.");

        // Lee el ProblemDetails (o ValidationProblemDetails) que devuelve la API
        private static async Task<ApiError> ReadErrorAsync(HttpResponseMessage response)
        {
            var status = (int)response.StatusCode;
            ProblemPayload? problem = null;

            var body = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    problem = JsonSerializer.Deserialize<ProblemPayload>(body, JsonOptions);
                }
                catch (JsonException)
                {
                    // El cuerpo no es JSON: se usa el mensaje genérico
                }
            }

            var details = problem?.Errors?.Values.SelectMany(messages => messages).ToList();

            var message = status switch
            {
                401 => "La API rechazó la autenticación (401). Revisa la configuración del cliente en Keycloak.",
                403 => "No tienes permiso para realizar esta operación (403).",
                404 when problem?.Detail is null => "La API no encontró el recurso o la ruta (404). Revisa ApiSettings:BaseUrl.",
                405 => "La API no permite ese método en esa ruta (405).",
                _ => problem?.Detail ?? problem?.Title ?? $"La API respondió con el código {status}."
            };

            return new ApiError(status, message, details);
        }

        private sealed class ProblemPayload
        {
            public string? Title { get; set; }
            public string? Detail { get; set; }
            public Dictionary<string, string[]>? Errors { get; set; }
        }
    }
}
