using Laserfiche.Oauth.Api.Client;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Laserfiche.OAuth.Client.ClientCredentials.IntegrationTest
{
    public class BaseTest
    {
        public ClientCredentialsOptions Configuration { get; set; }

        public BaseTest()
        {
            SourceTestConfig("TestConfig");
        }

        private void SourceTestConfig(string fileName)
        {
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, $"{fileName}.env");
            var env = DotNetEnv.Env.Load(path);

            Configuration = new();

            foreach (var kv in env)
            {
                switch (kv.Key)
                {
                    case "SERVICE_PRINCIPAL_KEY":
                        Configuration.ServicePrincipalKey = kv.Value;
                        break;
                    case "ACCESS_KEY":
                        Configuration.AccessKey = JsonConvert.DeserializeObject<AccessKey>(kv.Value);
                        break;
                }
            }
        }
    }
}
