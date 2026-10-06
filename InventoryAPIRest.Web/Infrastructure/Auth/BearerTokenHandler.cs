using System.Net.Http.Headers;

namespace InventoryAPIRest.Web.Infrastructure.Auth
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;
        public BearerTokenHandler(ITokenProvider tokenProvider) => _tokenProvider = tokenProvider;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
