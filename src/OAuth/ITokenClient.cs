using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.OAuth
{
    /// <summary>
    /// The token route API client.
    /// </summary>
    public partial interface ITokenClient
    {
        /// <summary>
        /// Gets an access token given the service principal key and the app access key. These values can be exported from the Laserfiche Developer Console. This is the client credentials flow that applies to service applications.
        /// </summary>
        /// <param name="servicePrincipalKey"></param>
        /// <param name="accessKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetAccessTokenResponse> GetAccessTokenFromServicePrincipalAsync(string servicePrincipalKey, AccessKey accessKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an access token given a jwt code, client id, redirect URI, client secret and code verifier. These values can be exported from the Laserfiche Developer Console. This is the client credentials flow that applies to service applications.
        /// </summary>
        /// <param name="code">Authorization code</param>
        /// <param name="redirectUri">Authorization endpoint redirect URI</param>
        /// <param name="clientId">OAuth application client ID</param>
        /// <param name="clientSecret">OPTIONAL OAuth application client secret. Required for web apps.</param>
        /// <param name="codeVerifier">OPTIONAL PKCE code verifier. Required for SPA apps.</param>
        /// <returns>A response that contains an access token</returns>
        Task<GetAccessTokenResponse> GetAccessTokenFromCode(string code, string redirectUri, string clientId, string clientSecret, string codeVerifier);

        /// <summary>
        /// Gets a refreshed access token given a refresh token. These values can be exported from the Laserfiche Developer Console. This is the client credentials flow that applies to service applications.
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="clientId">OAuth application client ID</param>
        /// <param name="clientSecret">OPTIONAL OAuth application client secret. Required for web apps.</param>
        /// <returns>A response that contains an access token</returns>

        Task<GetAccessTokenResponse> RefreshAccessToken(string refreshToken, string clientId, string clientSecret);

    }
}
