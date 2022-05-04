using Laserfiche.Oauth.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
{
    [TestClass]
    public class ValidationTests
    {
        private const string ACCOUNT_ID = "fake.account.id";
        private const string DOMAIN = "fake.domain";
        private const string CLIENT_ID = "fake.client.id";
        private const string SERVICE_PRINCIPAL_KEY = "fake.sp.key";
        private const string ACCESS_KEY = @"{
	            ""kty"": ""EC"",
                ""crv"": ""P-256"",
                ""use"": ""sig"",
	            ""kid"": ""YbcQaVGKoqiSmD2LwIrNRWk2y10oLYqDN5rymQyafwc"",
	            ""x"": ""oO6bmvSrJmSVzw72aJdKdH08Rw3LOKBsbN8-p9e-i2I"",
	            ""y"": ""TSg5da4l2ThYI__W34_3rLoUyoAZ-atb4cCELHTcstM"",
	            ""d"": ""Q2J9YzSI_p98uMlt-MvFAi5VkzcFzQ-ThE2VRtv1g-Y""
            }";

        [TestMethod]
        public void HandlerConfigurationMissingAccountId()
        {
            ClientCredentialsOptions config = new ClientCredentialsOptions()
            {
                Domain = DOMAIN,
                ClientId = CLIENT_ID,
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY,
                Jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(ACCESS_KEY)
            };
            Assert.ThrowsException<ArgumentException>(() => new TokenApiClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingDomain()
        {
            ClientCredentialsOptions config = new ClientCredentialsOptions()
            {
                CustomerId = ACCOUNT_ID,
                ClientId = CLIENT_ID,
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY,
                Jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(ACCESS_KEY)
            };
            Assert.ThrowsException<ArgumentException>(() => new TokenApiClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingClientId()
        {
            ClientCredentialsOptions config = new ClientCredentialsOptions()
            {
                CustomerId = ACCOUNT_ID,
                Domain = DOMAIN,
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY,
                Jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(ACCESS_KEY)
            };
            Assert.ThrowsException<ArgumentException>(() => new TokenApiClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingServicePrincipalKey()
        {
            ClientCredentialsOptions config = new ClientCredentialsOptions()
            {
                CustomerId = ACCOUNT_ID,
                Domain = DOMAIN,
                ClientId = CLIENT_ID,
                Jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(ACCESS_KEY)
            };
            Assert.ThrowsException<ArgumentException>(() => new TokenApiClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingAccesskey()
        {
            ClientCredentialsOptions config = new ClientCredentialsOptions()
            {
                CustomerId = ACCOUNT_ID,
                Domain = DOMAIN,
                ClientId = CLIENT_ID,
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY
            };
            Assert.ThrowsException<ArgumentException>(() => new TokenApiClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationCannotBeNull()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new TokenApiClient(null));
            Assert.AreEqual(ex.Message, new ArgumentNullException("configuration").Message);
        }
    }
}
