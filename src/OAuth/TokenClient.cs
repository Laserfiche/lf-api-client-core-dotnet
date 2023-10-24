// Copyright (c) Laserfiche
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.Utils;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.OAuth
{
    /// <summary>
    /// The API client for the Laserfiche Cloud token route.
    /// </summary>
    public partial class TokenClient : ITokenClient
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionalDomain">Laserfiche Cloud domain associated with the access key. Since the authorization service is region based, one cannot change the base address once it's set. This value can be found in AccessKey.Domain.</param>
        public TokenClient(string regionalDomain)
        {
            _httpClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false,
            })
            {
                BaseAddress = new Uri(DomainUtils.GetOAuthApiBaseUri(regionalDomain))
            };
            _settings = new Lazy<JsonSerializerSettings>(CreateSerializerSettings);
        }

        public async Task<GetAccessTokenResponse> GetAccessTokenFromServicePrincipalAsync(string servicePrincipalKey, AccessKey accessKey, string scope = null, CancellationToken cancellationToken = default)
        {
            if (!string.Equals(_httpClient.BaseAddress.AbsoluteUri, DomainUtils.GetOAuthApiBaseUri(accessKey.Domain), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentOutOfRangeException($"{nameof(accessKey)}.{nameof(accessKey.Domain)}");
            }

            string bearerAuth = $"Bearer {JwtUtils.CreateClientCredentialsAuthorizationJwt(servicePrincipalKey, accessKey)}";
            var response = await TokenAsync(new GetAccessTokenRequest()
            {
                Grant_type = "client_credentials",
                Scope = scope
            }, bearerAuth).ConfigureAwait(false);
            return response;
        }

        public async Task<GetAccessTokenResponse> GetAccessTokenFromCode(string code, string redirectUri, string clientId, string clientSecret = null, string codeVerifier = null, string scope = null)
        {
            string basicAuth = JwtUtils.CreateBasicAuth(clientId, clientSecret);
            var response = await TokenAsync(new GetAccessTokenRequest()
            {
                Grant_type = "authorization_code",
                Client_id = clientId,
                Code = code,
                Redirect_uri = redirectUri,
                Code_verifier = codeVerifier,
                Scope = scope
            }, basicAuth).ConfigureAwait(false);
            return response;
        }

        public async Task<GetAccessTokenResponse> RefreshAccessToken(string refreshToken, string clientId, string clientSecret = null)
        {
            string basicAuth = JwtUtils.CreateBasicAuth(clientId,clientSecret);
            var response = await TokenAsync(new GetAccessTokenRequest()
            {
                Grant_type = "refresh_token",
                Client_id = clientId,
                Refresh_token = refreshToken
            }, basicAuth).ConfigureAwait(false);
            return response;
        }

        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.MaxDepth = 128;
        }
    }
}
