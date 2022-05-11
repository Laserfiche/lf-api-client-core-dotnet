using Newtonsoft.Json;

namespace Laserfiche.Api.Client.Util
{
    internal class OAuthProblemDetails
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("instance")]
        public string Instance { get; set; }

        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Title}";
        }
    }
}
