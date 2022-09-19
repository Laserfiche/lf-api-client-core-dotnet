using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client.SelfHosted;

namespace Laserfiche.Api.Client.HttpHandlers
{
    /// <summary>
    /// Self-hosted username password handler to set the authorization header given username, password, grant type, repository ID and base URL.
    /// </summary>
    public class SelfHostedUsernamePasswordHandler : IHttpRequestHandler
    {
        private string _accessToken;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;
        private readonly string _repoID;
        private readonly string _grantType;
        private readonly ITokenClient _client;
        private readonly CreateConnectionRequest _request;

        public SelfHostedUsernamePasswordHandler(string username, string password, string grantType, string repoID, string baseUrl) : this(username, password, grantType, repoID, baseUrl, null) { }

        internal SelfHostedUsernamePasswordHandler(string username, string password, string grantType, string repoID, string baseUrl, ITokenClient client)
        {
            _username = username;
            _password = password;
            _baseUrl = baseUrl.TrimEnd('/') + "/";
            _repoID = repoID;
            _grantType = grantType;

            _request = new CreateConnectionRequest
            {
                Username = _username,
                Password = _password,
                Grant_type = _grantType
            };
            _client = client ?? new TokenClient(new HttpClient { BaseAddress = new Uri(_baseUrl) });
        }

        public async Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                var response = await _client.TokenAsync(_repoID, _request, cancellationToken);
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

