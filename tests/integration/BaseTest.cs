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

        private const string TestConfigFile = ".env";
        private const string AccessKeyVar = "ACCESS_KEY";
        private const string SpKeyVar = "SERVICE_PRINCIPAL_KEY";
        private const string UsernameVar = "LFDS_USERNAME";
        private const string PasswordVar = "LFDS_PASSWORD";
        private const string RepoIdVar = "REPOSITORY_ID";
        private const string OrganizationVar = "LFDS_ORGANIZATION";
        private const string BaseUrlVar = "SELFHOSTED_REPOSITORY_API_BASE_URI";

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

        private void PopulateFromEnv()
        {
            ServicePrincipalKey = Environment.GetEnvironmentVariable(SpKeyVar);
            AccessKey = AccessKey.DecodeBase64(Environment.GetEnvironmentVariable(AccessKeyVar));
            Username = Environment.GetEnvironmentVariable(UsernameVar);
            Password = Environment.GetEnvironmentVariable(PasswordVar);
            RepoId = Environment.GetEnvironmentVariable(RepoIdVar);
            Organization = Environment.GetEnvironmentVariable(OrganizationVar);
            BaseUrl = Environment.GetEnvironmentVariable(BaseUrlVar);
        }
    }
}
