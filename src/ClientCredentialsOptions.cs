using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Laserfiche.Oauth.Api.Client
{
    public class ClientCredentialsOptions : IClientCredentialsOptions
    {
        [JsonProperty("csid")]
        public string AccountId { set; get; } = string.Empty;

        [JsonProperty("domain")]
        public string Domain { set; get; } = string.Empty;

        [JsonProperty("client_id")]
        public string ClientId { set; get; } = string.Empty;

        [JsonProperty("servicePrincipalKey")]
        public string ServicePrincipalKey { set; get; } = string.Empty;

        [JsonProperty("accessKey")]
        public JsonWebKey AccessKey { set; get; } = null;
    }
}
