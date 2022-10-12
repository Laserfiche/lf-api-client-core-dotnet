using Newtonsoft.Json;

namespace Laserfiche.Api.Client.APIServer
{
    partial class TokenClient
    {
        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.MaxDepth = 128;
        }
    }
}
