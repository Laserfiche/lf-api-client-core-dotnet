using Laserfiche.Oauth.Api.Client.Util;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    // Built based on the TokenClient.
    public class OauthClientCredentialsHandler : IHttpRequestHandler
    {
        private string _accessToken;

        private readonly string _servicePrincipalKey;

        private readonly AccessKey _accessKey;

        private readonly ITokenApiClient _tokenApiClient;

        // Put ClientCredentialsClient stuff into here
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
        /// A BeforeSendAsync implementation that will automatically get an access token when one does not exist in the
        /// repository client.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
        /// An AfterSendAsync implementation that will automatically refresh an access token when the current access token
        /// expires.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
