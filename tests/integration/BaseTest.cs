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
            var testConfig = Environment.GetEnvironmentVariable("TEST_CONFIG_DEV_A_CA");
            
            // GitHub secret will be passed in as environment variable. If it doesn't exist,
            // we will fallback to reading the JSON file.
            if (testConfig == null) {
                ExtractConfigurationFromJson();
            } else {
                Configuration = JsonConvert.DeserializeObject<ClientCredentialsOptions>(testConfig);
            }
        }

        private void ExtractConfigurationFromJson()
        {
            string testingConfig;
            try
            {
                testingConfig = File.ReadAllText(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "DEV_A_CA.json"));
            }
            catch (Exception)
            {
                throw new Exception("Cannot load TestingConfig.json");
            }
            Configuration = JsonConvert.DeserializeObject<ClientCredentialsOptions>(testingConfig);
        }
    }
}
