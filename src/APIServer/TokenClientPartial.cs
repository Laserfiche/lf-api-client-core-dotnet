using Newtonsoft.Json;

namespace Laserfiche.Api.Client.APIServer
{
    /// <summary>
    /// The Laserfiche Self-Hosted token route API client.
    /// </summary>
    public partial interface ITokenClient
    {
    }

    /// <summary>
    /// The Laserfiche Self-Hosted token route API client.
    /// </summary>
    partial class TokenClient
    {
        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.MaxDepth = 128;
        }
    }
}
