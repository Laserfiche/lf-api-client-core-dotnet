using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    public interface ITokenApiClient
    {
        /// <summary>
        /// Gets a new oauth access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An TokenResponse object.</returns>
        Task<TokenResponse> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Refresh the oauth access token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An TokenResponse object.</returns>
        Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
