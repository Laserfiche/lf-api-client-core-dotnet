// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        /// <summary>
        /// The Laserfiche customer id the app is registered in.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { set; get; }

        /// <summary>
        /// The Laserfiche domain the app belongs to, e.g. laserfiche.com.
        /// </summary>
        [JsonProperty("domain")]
        public string Domain { set; get; }

        /// <summary>
        /// The app's client id.
        /// </summary>
        [JsonProperty("clientId")]
        public string ClientId { set; get; }

        /// <summary>
        /// The app's json web key.
        /// </summary>
        [JsonProperty("jwk")]
        public JsonWebKey Jwk { set; get; }

        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() { MaxDepth = 128 };

        /// <summary>
        /// Creates an AccessKey using the base-64 encoded access key associated with an OAuth service app,
        /// which can be exported from the Laserfiche Developer Console.
        /// </summary>
        /// <param name="base64EncodedAccessKey">The base-64 encoded access key exported from the Laserfiche Developer Console.</param>
        /// <returns>AccessKey</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static AccessKey CreateFromBase64EncodedAccessKey(string base64EncodedAccessKey)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedAccessKey))
            {
                throw new ArgumentNullException(nameof(base64EncodedAccessKey));
            }
            var accessKeyStr = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedAccessKey));
            try
            {
                AccessKey accessKey = JsonConvert.DeserializeObject<AccessKey>(accessKeyStr, jsonSerializerSettings);
                return accessKey;
            }
            catch(JsonReaderException e)
            {
                throw new FormatException($"{nameof(base64EncodedAccessKey)} is not valid", e);
            }
        }
    }
}
