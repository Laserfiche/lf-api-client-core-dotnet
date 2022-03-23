using Microsoft.IdentityModel.JsonWebTokens;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    public interface IClientCredentialsHandler
    {
        IClientCredentialsOptions Configuration { set; get; }
        /// <summary>
        /// Get access token as string
        /// </summary>
        /// <returns></returns>
        Task<string> GetAccessToken();
        /// <summary>
        /// Get access token in JsonWebToken format
        /// </summary>
        /// <returns></returns>
        Task<JsonWebToken> GetAccessTokenAsJWT();
    }
}
