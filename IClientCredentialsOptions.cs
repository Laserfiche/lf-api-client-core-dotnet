using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Laserfiche.OAuth.Client.ClientCredentials
{
    public interface IClientCredentialsOptions
    {
        string BaseAddress { set; get; }

        string ClientId { set; get; }

        string ServicePrincipalKey { set; get; }

        JsonWebKey SigningKey { set; get; }

        (bool, List<string>) IsValid();
    }
}
