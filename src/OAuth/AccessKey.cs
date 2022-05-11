using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Laserfiche.Api.Client.OAuth
{
    /// <summary>
    /// Access key together with the service principal key are the secrets associated with an OAuth service app,
    /// which can be exported from the Laserfiche Developer Console.
    /// </summary>
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
