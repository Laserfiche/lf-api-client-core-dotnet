using Laserfiche.Api.Client.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Laserfiche.OAuth.Client.ClientCredentials.IntegrationTest
{
    public class BaseTest
    {
        public AccessKey AccessKey { get; set; }

        public string ServicePrincipalKey { get; set; }

        private const string SP_KEY = "SERVICE_PRINCIPAL_KEY";

        private const string ACCESS_KEY = "ACCESS_KEY";

        private const string ENV_TEST_CONFIG = "TEST_CONFIG";

        public BaseTest()
        {
            if (Environment.GetEnvironmentVariable(ENV_TEST_CONFIG) == null)
            {
                SourceTestConfigFromFile("TestConfig");
            } else
            {
                var config = Environment.GetEnvironmentVariable(ENV_TEST_CONFIG);
                var env = DotNetEnv.Env.LoadContents(config);
                CreateAndPopulateTestConfig(env);
            }
        }

        private void SourceTestConfigFromFile(string fileName)
        {
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, $"{fileName}.env");
            var env = DotNetEnv.Env.Load(path);
            CreateAndPopulateTestConfig(env);
        }

        private void CreateAndPopulateTestConfig(IEnumerable<KeyValuePair<string, string>> env)
        {
            foreach (var kv in env)
            {
                switch (kv.Key)
                {
                    case SP_KEY:
                        ServicePrincipalKey = kv.Value;
                        break;
                    case ACCESS_KEY:
                        AccessKey = JsonConvert.DeserializeObject<AccessKey>(kv.Value);
                        break;
                }
            }
        }
    }
}
