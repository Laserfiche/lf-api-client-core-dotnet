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
            ExtractConfigurationFromJson();
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
