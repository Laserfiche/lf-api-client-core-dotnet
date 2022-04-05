using Microsoft.IdentityModel.Tokens;

namespace Laserfiche.Oauth.Api.Client
{
    public interface IClientCredentialsOptions
    {
        /// <summary>
        /// The Laserfiche account the application is registered in.
        /// </summary>
        string AccountId { set; get; }

        /// <summary>
        /// The Laserfiche domain the application is registered in.
        /// </summary>
        string Domain { set; get; }

        /// <summary>
        /// The client id of the application. 
        /// </summary>
        string ClientId { set; get; }

        /// <summary>
        /// The service principal key for the service principal assigned to the application.
        /// </summary>
        string ServicePrincipalKey { set; get; }

        /// <summary>
        /// The access key of the application.
        /// </summary>
        JsonWebKey AccessKey { set; get; }
    }
}
