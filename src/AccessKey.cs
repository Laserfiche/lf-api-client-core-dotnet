using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Laserfiche.Oauth.Api.Client
{
    public class AccessKey
    {
        [JsonProperty("customerId")]
        public string CustomerId { set; get; }

        [JsonProperty("domain")]
        public string Domain { set; get; }

        [JsonProperty("clientId")]
        public string ClientId { set; get; }

        [JsonProperty("jwk")]
        public JsonWebKey Jwk { set; get; }
    }
}
