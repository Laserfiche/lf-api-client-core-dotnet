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
    public class ClientCredentialsHandler : IClientCredentialsHandler
    {
        public HttpClient HttpClient { get; private set; }

        public IClientCredentialsOptions Configuration { set; get; }

        public static async Task<IClientCredentialsHandler> CreateFromAccessKeyAsync(string accessKeyFilePath, IHttpClientFactory httpClientFactory = null)
        {
            using (FileStream fileStream = File.Open(accessKeyFilePath, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string content = await streamReader.ReadToEndAsync();
                    var configuration = JsonConvert.DeserializeObject<ClientCredentialsOptions>(content);
                    return new ClientCredentialsHandler(configuration, httpClientFactory);
                }
            }
        }

        public ClientCredentialsHandler(IClientCredentialsOptions configuration, IHttpClientFactory httpClientFactory = null)
        {
            SetupClientCredentialsHandler(configuration, httpClientFactory);
        }

        private void SetupClientCredentialsHandler(IClientCredentialsOptions configuration, IHttpClientFactory httpClientFactory = null)
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
                HttpClient = httpClientFactory.CreateClient();
            }
            else
            {
                HttpClient = new HttpClient(new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    UseCookies = false,
                });
            }
        }

        public async Task<(string accessToken, string refreshToken)> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            return await RequestAccessTokenAsync(cancellationToken);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await RequestAccessTokenAsync(cancellationToken);
        }

        private async Task<(string accessToken, string refreshToken)> RequestAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, DomainUtil.GetOauthTokenUri(Configuration.Domain))
            {
                Content = ClientCredentialsUtil.RequestToFormUrlEncodedContent(new ClientCredentialsTokenRequest() { })
            };
            var authHeader = $"Bearer {JwtUtil.CreateClientCredentialsAuthorizationJwt(Configuration)}";
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var oauthResponse = JsonConvert.DeserializeObject<OAuthResponse>(content);
                return (oauthResponse.AccessToken, string.Empty);
            }
            else
            {
                throw ClientCredentialsUtil.GetStandardErrorException(content, response.StatusCode);
            }
        }
    }
}
