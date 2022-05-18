using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.OAuth
{
    /// <summary>
    /// The token route API client.
    /// </summary>
    public partial interface ITokenApiClient
    {
        /// <summary>
        /// Gets an access token given the service principal key and the app access key. These values can be exported from the Laserfiche Developer Console. This is the client credentials flow that applies to service applications.
        /// </summary>
        /// <param name="servicePrincipalKey"></param>
        /// <param name="accessKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetAccessTokenResponse> GetAccessTokenAsync(string servicePrincipalKey, AccessKey accessKey, CancellationToken cancellationToken = default);
    }
}
