using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client.APIServer;

namespace Laserfiche.Api.Client.HttpHandlers
{
    /// <summary>
    /// Username password handler for self-hosted API Server to set the authorization header given repository ID, username, password, and base URL.
    /// </summary>
    public class UsernamePasswordHandler : IHttpRequestHandler
    {
        private string _accessToken;
        private const string GrantType = "password";

        private readonly string _repoId;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;

        private readonly ITokenClient _client;
        private readonly CreateConnectionRequest _request;

        public UsernamePasswordHandler(string repoId, string username, string password, string baseUrl) : this(repoId, username, password, baseUrl, null) { }

        internal UsernamePasswordHandler(string repoId, string username, string password, string baseUrl, ITokenClient client)
        {
            _username = username;
            _password = password;
            _baseUrl = baseUrl.TrimEnd('/') + "/";
            _repoId = repoId;

            _request = new CreateConnectionRequest
            {
                Username = _username,
                Password = _password,
                Grant_type = GrantType
            };
            _client = client ?? new TokenClient(new HttpClient { BaseAddress = new Uri(_baseUrl) });
        }

        public async Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                var response = await _client.TokenAsync(_repoId, _request, cancellationToken);
                _accessToken = response?.Access_token;
            }
            httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            return new BeforeSendResult();
        }

        public Task<bool> AfterSendAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                _accessToken = null; // In case exception happens when getting the access token.
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}

