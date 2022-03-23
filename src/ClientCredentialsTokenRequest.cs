using Newtonsoft.Json;

namespace Laserfiche.Oauth.Api.Client
{
    public class ClientCredentialsTokenRequest
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; } = "client_credentials";

        [JsonProperty("client_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ClientId { get; set; }

        [JsonProperty("client_secret", NullValueHandling = NullValueHandling.Ignore)]
        public string ClientSecret { get; set; }

        [JsonProperty("expires_in", NullValueHandling = NullValueHandling.Ignore)]
        public int ExpiresIn { get; set; } = 3600;

    }
}
