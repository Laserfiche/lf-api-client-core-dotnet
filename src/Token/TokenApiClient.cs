using Laserfiche.Oauth.Api.Client.Util;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    /// <summary>
    /// API Client for the OAuth token route.
    /// </summary>
    public partial class TokenApiClient : ITokenApiClient
    {
        // The authorization endpoint should be global and well known. So, ClientCredentialsOption doesn't need to be passed in. But our current backend is reginal.
        public TokenApiClient(string regionalDomain)
        {
            _httpClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false,
            });
            _httpClient.BaseAddress = new Uri(DomainUtil.GetOauthBaseUri(regionalDomain));
        }

        public async Task<SwaggerResponse<GetAccessTokenResponse>> GetAccessTokenAsync(string servicePrincipalKey, AccessKey accessKey, CancellationToken cancellationToken = default)
        {
            string bearerAuth = $"Bearer {JwtUtil.CreateClientCredentialsAuthorizationJwt(servicePrincipalKey, accessKey)}";
            var response = await TokenAsync(new GetAccessTokenRequest()
            {
                Grant_type = "client_credentials",
            }, bearerAuth);
            return response;
        }
    }
}
