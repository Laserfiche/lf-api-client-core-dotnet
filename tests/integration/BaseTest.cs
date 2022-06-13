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

        public string Username { get; set; }
        public string Password { get; set; }
        public string RepoId { get; set; }
        public string Organization { get; set; }
        public string BaseUrl { get; set; }


        private const string TestConfigFile = "TestConfig.env";

        private const string AccessKeyVar = "ACCESS_KEY";

        private const string SpKeyVar = "SERVICE_PRINCIPAL_KEY";

        private const string usernameVar = "LFDS_TEST_USERNAME";

        private const string passwordVar = "LFDS_TEST_PASSWORD";

        private const string repoIdVar = "REPOSITORY_ID";

        private const string organizationVar = "LFDS_TEST_ORGANIZATION";

        private const string baseUrlVar = "SELFHOSTED_REPOSITORY_API_BASE_URI";

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
                System.Diagnostics.Trace.WriteLine($"{fileName} not found.");
        }

        private static string DecodeBase64(string encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }

        private void PopulateFromEnv()
        {
            ServicePrincipalKey = Environment.GetEnvironmentVariable(SpKeyVar);

            var accessKeyStr = DecodeBase64(Environment.GetEnvironmentVariable(AccessKeyVar));
            AccessKey = JsonConvert.DeserializeObject<AccessKey>(accessKeyStr);

            Username = Environment.GetEnvironmentVariable(usernameVar);
            Password = Environment.GetEnvironmentVariable(passwordVar);
            RepoId = Environment.GetEnvironmentVariable(repoIdVar);
            Organization = Environment.GetEnvironmentVariable(organizationVar);
            BaseUrl = Environment.GetEnvironmentVariable(baseUrlVar);
        }
    }
}
