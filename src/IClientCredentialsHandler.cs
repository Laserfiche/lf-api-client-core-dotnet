using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    public interface IClientCredentialsHandler
    {
        /// <summary>
        /// The configuration for the <see cref="IClientCredentialsHandler"/>.
        /// </summary>
        IClientCredentialsOptions Configuration { set; get; }

        /// <summary>
        /// Gets a new oauth access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An access token and a refresh token.</returns>
        Task<(string accessToken, string refreshToken)> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Refresh the oauth access token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An access token and a refresh token.</returns>
        Task<(string accessToken, string refreshToken)> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
