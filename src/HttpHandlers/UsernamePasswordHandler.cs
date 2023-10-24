// Copyright (c) Laserfiche
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client.APIServer;

namespace Laserfiche.Api.Client.HttpHandlers
{
    /// <summary>
    /// Username password handler for Laserfiche Self-Hosted API Server to set the authorization header given repository ID, username, password, and base URL.
    /// </summary>
    public class UsernamePasswordHandler : IHttpRequestHandler
    {
        private string _accessToken;
        private const string GrantType = "password";

        private readonly string _repositoryId;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;

        private readonly ITokenClient _client;
        private readonly CreateConnectionRequest _request;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositoryId">The repository ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="baseUrl">API server base URL e.g., https://{APIServerName}/LFRepositoryAPI.</param>
        public UsernamePasswordHandler(string repositoryId, string username, string password, string baseUrl) : this(repositoryId, username, password, baseUrl, null) { }

        internal UsernamePasswordHandler(string repositoryId, string username, string password, string baseUrl, ITokenClient client)
        {
            if (baseUrl == null)
                throw new ArgumentNullException(nameof(baseUrl));

            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _repositoryId = repositoryId ?? throw new ArgumentNullException(nameof(repositoryId));
            _baseUrl = baseUrl.TrimEnd('/') + "/";

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
                var response = await _client.TokenAsync(_repositoryId, _request, cancellationToken).ConfigureAwait(false);
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

