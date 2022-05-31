using Laserfiche.Api.Client.OAuth;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Laserfiche.Api.Client.IntegrationTest
{
    public class BaseTest
    {
        public AccessKey AccessKey { get; set; }

        public string ServicePrincipalKey { get; set; }

        private const string TestConfigFile = "TestConfig.env";

        public BaseTest()
        {
            TryLoadFromDotEnv(TestConfigFile);
            PopulateFromEnv();
        }

        private static void TryLoadFromDotEnv(string fileName)
        {
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, fileName);
            if (File.Exists(path))
            {
                DotNetEnv.Env.Load(path, new DotNetEnv.LoadOptions(
                    setEnvVars: true,
                    clobberExistingVars: true,
                    onlyExactPath: true
                ));
                System.Diagnostics.Trace.TraceWarning($"{fileName} found. {fileName} file should only be used in local developer computers.");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine($"{fileName} not found.");
            }
        }

        private void PopulateFromEnv()
        {
            ServicePrincipalKey = Environment.GetEnvironmentVariable("DEV_CA_PUBLIC_USE_TESTOAUTHSERVICEPRINCIPAL_SERVICE_PRINCIPAL_KEY");
            AccessKey = JsonConvert.DeserializeObject<AccessKey>(Environment.GetEnvironmentVariable("DEV_CA_PUBLIC_USE_INTEGRATION_TEST_ACCESS_KEY"));
        }
    }
}
