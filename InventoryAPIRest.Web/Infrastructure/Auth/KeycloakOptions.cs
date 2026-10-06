namespace InventoryAPIRest.Web.Infrastructure.Auth
{
    public class KeycloakOptions
    {
        public const string SectionName = "Keycloak";
        public string Authority { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string TokenEndpoint => $"{Authority.TrimEnd('/')}/protocol/openid-connect/token";
    }
}
