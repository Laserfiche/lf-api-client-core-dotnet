using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    // This is the client rest route (non-auto generated one)
    public partial interface ITokenApiClient
    {
        /// <summary>
        /// Gets a new oauth access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An TokenResponse object.</returns>
        Task<SwaggerResponse<GetAccessTokenResponse>> GetAccessTokenAsync(string servicePrincipalKey, AccessKey accessKey, CancellationToken cancellationToken = default);
    }
}
