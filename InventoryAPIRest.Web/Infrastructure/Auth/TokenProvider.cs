using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace InventoryAPIRest.Web.Infrastructure.Auth
{
    public interface ITokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    }
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
    internal sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);

    public class KeycloakTokenProvider : ITokenProvider
    {
        public const string HttpClientName = "keycloak";
        private static readonly TimeSpan SafetyMargin = TimeSpan.FromSeconds(30);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KeycloakOptions _options;
        private readonly SemaphoreSlim _gate = new(1, 1);
        private string? _token;
        private DateTime _expiresAtUtc = DateTime.MinValue;
        public KeycloakTokenProvider(IHttpClientFactory httpClientFactory, IOptions<KeycloakOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }
        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            if (HasValidToken()) return _token!;

            await _gate.WaitAsync(cancellationToken);
            try
            {
                if (HasValidToken()) return _token!;

                if (string.IsNullOrWhiteSpace(_options.ClientSecret))
                    throw new AuthenticationFailedException(
                        "Falta 'Keycloak:ClientSecret'. Configúralo con user-secrets o con la variable de entorno Keycloak__ClientSecret.");

                var client = _httpClientFactory.CreateClient(HttpClientName);
                var form = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _options.ClientId,
                    ["client_secret"] = _options.ClientSecret
                });

                using var response = await client.PostAsync(_options.TokenEndpoint, form, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    throw new AuthenticationFailedException(
                        $"Keycloak respondió {(int)response.StatusCode} al solicitar el token. Verifica ClientId y ClientSecret.");

                var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
                    ?? throw new AuthenticationFailedException("Keycloak devolvió una respuesta vacía.");

                _token = payload.AccessToken;
                _expiresAtUtc = DateTime.UtcNow.AddSeconds(payload.ExpiresIn) - SafetyMargin;
                return _token;
            }
            catch (HttpRequestException ex)
            {
                throw new AuthenticationFailedException("No se pudo contactar a Keycloak.", ex);
            }
            finally
            {
                _gate.Release();
            }
        }
        private bool HasValidToken() => _token is not null && DateTime.UtcNow < _expiresAtUtc;
    }
}
