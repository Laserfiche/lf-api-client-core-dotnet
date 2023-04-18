using Laserfiche.Api.Client.OAuth;
using Laserfiche.Api.Client.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.HttpHandlers
{
    /// <summary>
    /// Laserfiche Cloud OAuth client credentials handler to set the authorization header JWT given an access key and a service principal key associated with an OAuth service app.
    /// </summary>
    public class OAuthClientCredentialsHandler : IHttpRequestHandler
    {
        private string _accessToken;

        private readonly string _servicePrincipalKey;

        private readonly AccessKey _accessKey;

        private readonly ITokenClient _tokenClient;

        private readonly string _scope;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="servicePrincipalKey">The service principal key created for the service principal from the Laserfiche Account Administration.</param>
        /// <param name="accessKey">The access key exported from the Laserfiche Developer Console.</param>
        /// <param name="scope">(optional) Specifies the request scopes for the authorization request. Scopes are case-sensitive and space delimited.</param>
        public OAuthClientCredentialsHandler(string servicePrincipalKey, AccessKey accessKey, string scope = null)
        {
            _servicePrincipalKey = servicePrincipalKey;
            if (string.IsNullOrEmpty(_servicePrincipalKey))
            {
                throw new ArgumentException(Resources.Strings.INVALID_SERVICE_PRINCIPAL_KEY, nameof(servicePrincipalKey));
            }

            _accessKey = accessKey;
            _accessKey.IsValid();
            _tokenClient = new TokenClient(_accessKey.Domain);
            _scope = scope;
        }

        /// <summary>
        /// Invoked before an HTTP request with the request message and cancellation token.
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>BeforeSendResult</returns>
        public async Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                var response = await _tokenClient.GetAccessTokenFromServicePrincipalAsync(_servicePrincipalKey, _accessKey, _scope, cancellationToken).ConfigureAwait(false);
                _accessToken = response.Access_token;
            }

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            // Will create a BeforeSendResult class and return it 
            return new BeforeSendResult() { RegionalDomain = _accessKey.Domain };
        }

        /// <summary>
        /// Invoked after an HTTP request with the response message and cancellation token.
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if the request should be retried.</returns>
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
