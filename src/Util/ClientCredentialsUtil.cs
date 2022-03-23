using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client.Util
{
    public static class ClientCredentialsUtil
    {
        internal static FormUrlEncodedContent RequestToFormUrlEncodedContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var kvp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return new FormUrlEncodedContent(kvp);
        }

        internal static async Task<dynamic> ResponseToDynamic(HttpResponseMessage response, JsonSerializerSettings settings = null)
        {
            var content = await response.Content.ReadAsStringAsync();
            dynamic data = settings != null ? JsonConvert.DeserializeObject(content, settings) : JsonConvert.DeserializeObject(content);
            return data;
        }
    }
}
