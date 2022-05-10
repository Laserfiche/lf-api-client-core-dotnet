using Laserfiche.Oauth.Api.Client.Util;
using Laserfiche.Oauth.Token.Client;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

// update packages name, github repo name, namespaces => Laserfiche.Api.Client.Core
namespace Laserfiche.Oauth.Api.Client
{
    /// <summary>
    /// OAuth client credentials handler to set the authorization header JWT given access key and service principal key.
    /// </summary>
    public class OauthClientCredentialsHandler : IHttpRequestHandler
    {
        private string _accessToken;

        private readonly string _servicePrincipalKey;

        private readonly AccessKey _accessKey;

        private readonly ITokenApiClient _tokenApiClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="servicePrincipalKey"></param>
        /// <param name="accessKey"></param>
        public OauthClientCredentialsHandler(string servicePrincipalKey, AccessKey accessKey)
        {
            _servicePrincipalKey = servicePrincipalKey;
            if (string.IsNullOrEmpty(_servicePrincipalKey))
            {
                throw new ArgumentException(Resources.Strings.INVALID_SERVICE_PRINCIPAL_KEY, nameof(_servicePrincipalKey));
            }

            _accessKey = accessKey;
            _accessKey.IsValid();

            _tokenApiClient = new TokenApiClient(_accessKey.Domain); // change the api so this one works
        }

        /// <summary>
        /// Invoked before an HTTP request with the request message and cancellation token.
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>BeforeSendResult</returns>
        public async Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                var response = await _tokenApiClient.GetAccessTokenAsync(_servicePrincipalKey, _accessKey, cancellationToken);
                _accessToken = response.Result.Access_token;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            // Will create a BeforeSendResult class and return it 
            return new BeforeSendResult() { RegionalDomain = _accessKey.Domain };
        }

        /// <summary>
        /// Invoked after a request with the response message and cancellation token.
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if the request should be retried.</returns>
        public Task<bool> AfterSendAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _accessToken = null; // In case exception happens when getting the access token.
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
