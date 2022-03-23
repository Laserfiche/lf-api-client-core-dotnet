using Laserfiche.OAuth.Client.ClientCredentials.Util;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Laserfiche.OAuth.Client.ClientCredentials
{
    public class ClientCredentialsHandler : IClientCredentialsHandler
    {
        private HttpClient _httpClient;

        private readonly ILogger _logger;

        public IClientCredentialsOptions Configuration { set; get; }

        public ClientCredentialsHandler(IClientCredentialsOptions configuration, IHttpClientFactory httpClientFactory = null, ILoggerFactory loggerFactory = null)
        {
            _logger = loggerFactory?.CreateLogger(this.GetType().FullName);

            if (configuration == null)
            {
                var nullConfigException = new ArgumentNullException(nameof(configuration));
                _logger?.LogError(nullConfigException.Message);
                throw nullConfigException;
            }

            var (isValid, whyInvalid) = configuration.IsValid();
            if (!isValid)
            {
                var invalidConfigException = new ArgumentException(string.Join("\n", whyInvalid));
                _logger?.LogError(invalidConfigException.Message);
                throw invalidConfigException;
            }
            Configuration = configuration;
            
            if (httpClientFactory != null)
            {
                _httpClient = httpClientFactory.CreateClient();
            } else
            {
                ConfigureHttpClient();
            }
        }
        
        private void ConfigureHttpClient()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient(new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    UseCookies = false,
                });
            }
        }

        public async Task<string> GetAccessToken()
        {
            // Generate auth header
            var authHeader = $"Bearer {JwtUtil.CreateClientCredentialsAuthorizationJwt(Configuration)}";

            // Get token from oauth
            var jwt = await RequestAccessToken(authHeader);

            return jwt;
        }

        public async Task<JsonWebToken> GetAccessTokenAsJWT()
        {
            string access_token = await GetAccessToken();
            var jwt = JwtUtil.ReadJWT(access_token);
            return jwt;
        }

        private async Task<string> RequestAccessToken(string authHeader)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, OAuthUtil.GetOauthUri("Token", Configuration))
            {
                Content = ClientCredentialsUtil.RequestToFormUrlEncodedContent(new ClientCredentialsTokenRequest() {})
            };
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
            var response = await _httpClient.SendAsync(request);
            var data = await ClientCredentialsUtil.ResponseToDynamic(response);

            var json = data as JObject;
            if (json != null)
            {
                var status = string.Empty;
                if (json["status"] != null)
                {
                    status = json["status"].Value<string>();
                }

                if (status == "401")
                {
                    var ex401 = new ArgumentException(Resources.Strings.ACCESS_TOKEN_RETRIEVAL_FAILED_401);
                    _logger?.LogError(ex401.Message);
                    throw ex401;
                } else if (status == "404")
                {
                    var ex404 = new ArgumentException(Resources.Strings.ACCESS_TOKEN_RETRIEVAL_FAILED_404);
                    _logger?.LogError(ex404.Message);
                    throw ex404;
                }
            }

            return (string)data.access_token;
        }
    }
}
