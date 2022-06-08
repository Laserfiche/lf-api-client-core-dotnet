using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client.Lfds;

namespace Laserfiche.Api.Client.HttpHandlers
{
    /// <summary>
    /// LFDS username password handler to set the authorization header given username, password, organization, repository ID and base URL.
    /// </summary>
    public class LfdsUsernamePasswordHandler : IHttpRequestHandler
    {
        private string _accessToken;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;
        private readonly string _repoID;
        private readonly string _organization;

        public LfdsUsernamePasswordHandler(string username, string password, string organization, string repoID, string baseUrl)
        {
            _username = username;
            _password = password;
            _baseUrl = baseUrl.TrimEnd('/');
            _repoID = repoID;
            _organization = organization;
        }

        public async Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                var client = new AccessTokensApiClient(new HttpClient { BaseAddress = new Uri(_baseUrl)});
                var request = new CreateConnectionRequest
                {
                    Username = _username,
                    Password = _password,
                    Organization = _organization
                };
                var response = await client.CreateAsync(_repoID, request);
                _accessToken = response.AuthToken;
            }
            httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            return new BeforeSendResult() { SelfHostedBaseUrl = _baseUrl };
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

