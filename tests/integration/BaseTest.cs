using Laserfiche.Api.Client.OAuth;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Laserfiche.Api.Client.IntegrationTest
{
    public class BaseTest
    {
        public AccessKey AccessKey { get; set; }

        public string ServicePrincipalKey { get; set; }

        private const string TestConfigFile = "TestConfig.env";

        private const string AccessKeyVar = "DEV_CA_PUBLIC_USE_INTEGRATION_TEST_ACCESS_KEY";

        private const string SpKeyVar = "DEV_CA_PUBLIC_USE_TESTOAUTHSERVICEPRINCIPAL_SERVICE_PRINCIPAL_KEY";

        private bool HasBase64Decoded = false;

        public BaseTest()
        {
            TryLoadFromDotEnv(TestConfigFile);
            PopulateFromEnv();
        }

        private void TryLoadFromDotEnv(string fileName)
        {
            var path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, fileName);
            if (File.Exists(path))
            {
                DotNetEnv.Env.Load(path, new DotNetEnv.LoadOptions(
                    setEnvVars: true,
                    clobberExistingVars: true,
                    onlyExactPath: true
                ));

                // Decode ACCESS_KEY
                DecodeAccessKeyAndUpdateEnvVar();
                
                System.Diagnostics.Trace.TraceWarning($"{fileName} found. {fileName} file should only be used in local developer computers.");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine($"{fileName} not found.");
            }
        }

        private void DecodeAccessKeyAndUpdateEnvVar()
        {
            var base64Encoded = Environment.GetEnvironmentVariable(AccessKeyVar);
            if (base64Encoded != null)
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64Encoded));
                Environment.SetEnvironmentVariable(AccessKeyVar, decoded);
                HasBase64Decoded = true;
            }
        }

        private void PopulateFromEnv()
        {
            ServicePrincipalKey = Environment.GetEnvironmentVariable(SpKeyVar);
            
            if (!HasBase64Decoded)
            {
                DecodeAccessKeyAndUpdateEnvVar();
            }
            AccessKey = JsonConvert.DeserializeObject<AccessKey>(Environment.GetEnvironmentVariable(AccessKeyVar));
        }
    }
}
