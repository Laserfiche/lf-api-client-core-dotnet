using Laserfiche.Oauth.Api.Client.Util;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Token.Client
{
    /// <summary>
    /// The token route API client.
    /// </summary>
    public partial class TokenApiClient : ITokenApiClient
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionalDomain">Laserfiche Cloud domain associated with the access key. Since the authorization service is region based, one cannot change the base address once it's set. This value can be found in AccessKey.Domain.</param>
        public TokenApiClient(string regionalDomain)
        {
            _httpClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false,
            });
            _httpClient.BaseAddress = new Uri(DomainUtil.GetOauthBaseUri(regionalDomain));
        }

        /// <summary>
        /// Gets an access token given the service principal key and the app access key. These values can be exported from the Laserfiche Developer Console. This is the client credentials flow that applies to service applications.
        /// </summary>
        /// <param name="servicePrincipalKey"></param>
        /// <param name="accessKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SwaggerResponse<GetAccessTokenResponse>> GetAccessTokenAsync(string servicePrincipalKey, AccessKey accessKey, CancellationToken cancellationToken = default)
        {
            if (string.Equals(_httpClient.BaseAddress.Host, accessKey.Domain, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentOutOfRangeException($"{nameof(accessKey)}.{nameof(accessKey.Domain)}");
            }
            
            string bearerAuth = $"Bearer {JwtUtil.CreateClientCredentialsAuthorizationJwt(servicePrincipalKey, accessKey)}";
            var response = await TokenAsync(new GetAccessTokenRequest()
            {
                Grant_type = "client_credentials",
            }, bearerAuth);
            return response;
        }
    }
}
