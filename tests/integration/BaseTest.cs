using Laserfiche.Oauth.Api.Client;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Laserfiche.OAuth.Client.ClientCredentials.IntegrationTest
{
    public class BaseTest
    {
        public ClientCredentialsOptions Configuration { get; set; }

        private readonly string ACCESS_KEY = "TEST_CONFIG_DEV_A_CA";

        private readonly string SERVICE_PRINCIPAL = "TEST_CONFIG_SP_DEV_A_CA";

        public BaseTest()
        {
            var servicePrincipal = Environment.GetEnvironmentVariable(SERVICE_PRINCIPAL);
            if (servicePrincipal == null)
            {
                servicePrincipal = ReadJsonFromFile<ServicePrincipalConfig>(SERVICE_PRINCIPAL).Value;
            }

            AccessKey accessKey;
            var accessKeyStr = Environment.GetEnvironmentVariable(ACCESS_KEY);
            if (accessKeyStr == null)
            {
                accessKey = ReadJsonFromFile<AccessKey>(ACCESS_KEY);
            } else
            {
                accessKey = JsonConvert.DeserializeObject<AccessKey>(accessKeyStr);
            }

            Configuration = new()
            {
                ServicePrincipalKey = servicePrincipal,
                AccessKey = accessKey
            };
        }
        private static T ReadJsonFromFile<T>(string fileName)
        {
            string config;
            try
            {
                config = File.ReadAllText(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, $"{fileName}.json"));
            }
            catch (Exception)
            {
                throw new Exception($"Cannot load {fileName}");
            }
            return JsonConvert.DeserializeObject<T>(config);
        }
    }

    internal class ServicePrincipalConfig
    {
        public string Value { get; set; }
    }
}
