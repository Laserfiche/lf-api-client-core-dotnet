using Laserfiche.Oauth.Api.Client.Util;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        /// <summary>
        /// Note, the client credentials flow doesn't have a concept of refresh token. But the user can still pass in a
        /// refresh token, e.g., from the previous result of calling GetAccessTokenAsync
        /// (i.e., TokenResponse.RefreshToken). TokenResponse.RefreshToken will be null but since this method doesn't
        /// even read that value, we still maintain a uniformity of "using refresh token to get access token" procedure.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SwaggerResponse<GetAccessTokenResponse>> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var response = await TokenAsync(new GetAccessTokenRequest()
            {
                Grant_type = "client_credentials",
            }); // TODO: we will need to find a way to pass in the auth header somewhere which can be drived from spKey and accessKey
            return response;
        }

    }
}
