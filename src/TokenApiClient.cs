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
    public class TokenApiClient : ITokenApiClient
    {
        private HttpClient _httpClient { get; set; }

        public ClientCredentialsOptions Configuration { set; get; }

        public static async Task<TokenApiClient> CreateFromAccessKeyAsync(string accessKeyFilePath, IHttpClientFactory httpClientFactory = null)
        {
            using (FileStream fileStream = File.Open(accessKeyFilePath, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string content = await streamReader.ReadToEndAsync();
                    var configuration = JsonConvert.DeserializeObject<ClientCredentialsOptions>(content);
                    return new TokenApiClient(configuration, httpClientFactory);
                }
            }
        }

        public TokenApiClient(ClientCredentialsOptions configuration, IHttpClientFactory httpClientFactory = null)
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
