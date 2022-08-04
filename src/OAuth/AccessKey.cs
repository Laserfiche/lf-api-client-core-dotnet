using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Text;

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

        public static AccessKey CreateFromBase64EncodedAccessKey(string base64EncodedAccessKey)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedAccessKey))
            {
                throw new ArgumentNullException(nameof(base64EncodedAccessKey));
            }
            var accessKeyStr = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedAccessKey));
            try
            {
                AccessKey accessKey = JsonConvert.DeserializeObject<AccessKey>(accessKeyStr);
                return accessKey;
            }
            catch(JsonReaderException e)
            {
                throw new FormatException($"{nameof(base64EncodedAccessKey)} is not valid", e);
            }
        }
    }
}
