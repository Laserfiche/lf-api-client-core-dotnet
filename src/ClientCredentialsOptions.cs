using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Laserfiche.Oauth.Api.Client
{
    public class ClientCredentialsOptions
    {
        [JsonProperty("csid")]
        public string CustomerId { set; get; }

        [JsonProperty("domain")]
        public string Domain { set; get; }

        [JsonProperty("client_id")]
        public string ClientId { set; get; }

        [JsonProperty("servicePrincipalKey")]
        public string ServicePrincipalKey { set; get; }

        [JsonProperty("accessKey")]
        public JsonWebKey Jwk { set; get; }
    }
}
