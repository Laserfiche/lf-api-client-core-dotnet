using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Laserfiche.OAuth.Client.ClientCredentials
{
    public class ClientCredentialsOptions : IClientCredentialsOptions
    {
        [JsonProperty]
        public string BaseAddress { set; get; } = string.Empty;

        [JsonProperty]
        public string ClientId { set; get; } = string.Empty;

        [JsonProperty]
        public string ServicePrincipalKey { set; get; } = string.Empty;

        [JsonProperty]
        public JsonWebKey SigningKey { set; get; } = null;

        public (bool, List<string>) IsValid()
        {
            var isValid = true;
            var whyInvalid = new List<string>();
            
            if (BaseAddress == string.Empty)
            {
                whyInvalid.Add(Resources.Strings.INVALID_BASE_ADDRESS);
                isValid = false;
            }

            if (ClientId == string.Empty)
            {
                whyInvalid.Add(Resources.Strings.INVALID_CLIENT_ID);
                isValid = false;
            }

            if (ServicePrincipalKey == string.Empty)
            {
                whyInvalid.Add(Resources.Strings.INVALID_SERVICE_PRINCIPAL_KEY);
                isValid = false;
            }

            if (!ValidateSigningKey())
            {
                whyInvalid.Add(Resources.Strings.INVALID_SIGNING_KEY);
                isValid = false;
            }

            return (isValid, whyInvalid);
        }

        private bool ValidateSigningKey()
        {
            return SigningKey != null && SigningKey.KeySize != 0;
        }
    }
}
