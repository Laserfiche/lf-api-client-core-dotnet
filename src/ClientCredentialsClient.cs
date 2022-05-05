using Laserfiche.Oauth.Api.Client.Util;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    public class ClientCredentialsClient : ITokenApiClient
    {
        private HttpClient _httpClient { get; set; }

        public ClientCredentialsOptions Configuration { set; get; }

        public static async Task<ClientCredentialsClient> CreateFromAccessKeyAsync(string accessKeyFilePath, IHttpClientFactory httpClientFactory = null)
        {
            using (FileStream fileStream = File.Open(accessKeyFilePath, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string content = await streamReader.ReadToEndAsync();
                    var configuration = JsonConvert.DeserializeObject<ClientCredentialsOptions>(content);
                    return new ClientCredentialsClient(configuration, httpClientFactory);
                }
            }
        }

        public ClientCredentialsClient(ClientCredentialsOptions configuration, IHttpClientFactory httpClientFactory = null)
        {
            SetupClientCredentialsHandler(configuration, httpClientFactory);
        }

        private void SetupClientCredentialsHandler(ClientCredentialsOptions configuration, IHttpClientFactory httpClientFactory = null)
        {
            if (configuration == null)
            {
                var nullConfigException = new ArgumentNullException(nameof(configuration));
                throw nullConfigException;
            }

            configuration.IsValid();
            Configuration = configuration;

            if (httpClientFactory != null)
            {
                _httpClient = httpClientFactory.CreateClient();
            }
            else
            {
                _httpClient = new HttpClient(new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    UseCookies = false,
                });
            }
        }

        public async Task<TokenResponse> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            return await RequestAccessTokenAsync(cancellationToken);
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
        public async Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await RequestAccessTokenAsync(cancellationToken);
        }

        private async Task<TokenResponse> RequestAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, DomainUtil.GetOauthTokenUri(Configuration.Domain))
            {
                Content = ClientCredentialsUtil.RequestToFormUrlEncodedContent(new ClientCredentialsTokenRequest() { })
            };
            var authHeader = $"Bearer {JwtUtil.CreateClientCredentialsAuthorizationJwt(Configuration)}";
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<TokenResponse>(content);
            }
            else
            {
                throw ClientCredentialsUtil.GetStandardErrorException(content, response.StatusCode);
            }
        }
    }
}
