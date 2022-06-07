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

namespace Laserfiche.Api.Client.HttpHandlers
{
    public class LfdsUsernamePasswordHandler : IHttpRequestHandler
    {
        private string _accessToken;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUri;
        private readonly string _repoID;
        private readonly string _organization;
        private readonly HttpClient _credentialsClient;

        public LfdsUsernamePasswordHandler(string username, string password, string organization, string repoID, string baseUri, HttpClient client = null, string token = null)
        {
            _username = username;
            _password = password;
            _baseUri = baseUri;
            _repoID = repoID;
            _organization = organization;
            _credentialsClient = client?? new HttpClient();
            _accessToken = token ?? null;
        }

        public async Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}/v1-alpha/Repositories/{_repoID}/AccessTokens/Create"))
                {
                    string json = $"{{\"username\":\"{_username}\",\"password\":\"{_password}\",\"organization\":\"{_organization}\"}}";
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _credentialsClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, new CancellationToken());

                    var contextText = await response.Content.ReadAsStringAsync();
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(contextText);
                    dic.TryGetValue("authToken", out _accessToken);
                }
            }
            httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            return new BeforeSendResult() { SelfHostDomain = _baseUri };
        }

        Task<bool> IHttpRequestHandler.AfterSendAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
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
