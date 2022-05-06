using Laserfiche.Oauth.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
{
    [TestClass]
    public class ValidationTests
    {
        private const string CUSTOMER_ID = "fake.account.id";
        private const string DOMAIN = "fake.domain";
        private const string CLIENT_ID = "fake.client.id";
        private const string SERVICE_PRINCIPAL_KEY = "fake.sp.key";
        private const string JWK = @"{
	            ""kty"": ""EC"",
                ""crv"": ""P-256"",
                ""use"": ""sig"",
	            ""kid"": ""YbcQaVGKoqiSmD2LwIrNRWk2y10oLYqDN5rymQyafwc"",
	            ""x"": ""oO6bmvSrJmSVzw72aJdKdH08Rw3LOKBsbN8-p9e-i2I"",
	            ""y"": ""TSg5da4l2ThYI__W34_3rLoUyoAZ-atb4cCELHTcstM"",
	            ""d"": ""Q2J9YzSI_p98uMlt-MvFAi5VkzcFzQ-ThE2VRtv1g-Y""
            }";

        [TestMethod]
        public void HandlerConfigurationMissingCustomerId()
        {
            ClientCredentialsOptions config = new()
            {
                AccessKey = new()
                {
                    Domain = DOMAIN,
                    ClientId = CLIENT_ID,
                    Jwk = new JsonWebKey(JWK)
                },
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY,
            };
            Assert.ThrowsException<ArgumentException>(() => new ClientCredentialsClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingDomain()
        {
            ClientCredentialsOptions config = new()
            {
                AccessKey = new()
                {
                    CustomerId = CUSTOMER_ID,
                    ClientId = CLIENT_ID,
                    Jwk = new JsonWebKey(JWK)
                },
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY,
            };
            Assert.ThrowsException<ArgumentException>(() => new ClientCredentialsClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingClientId()
        {
            ClientCredentialsOptions config = new()
            {
                AccessKey = new()
                {
                    Domain = DOMAIN,
                    ClientId = CLIENT_ID,
                    Jwk = new JsonWebKey(JWK)
                },
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY,
            };
            Assert.ThrowsException<ArgumentException>(() => new ClientCredentialsClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingServicePrincipalKey()
        {
            ClientCredentialsOptions config = new()
            {
                AccessKey = new()
                {
                    CustomerId = CUSTOMER_ID,
                    Domain = DOMAIN,
                    ClientId = CLIENT_ID,
                    Jwk = new JsonWebKey(JWK)
                },
            };
            Assert.ThrowsException<ArgumentException>(() => new ClientCredentialsClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationMissingAccesskey()
        {
            ClientCredentialsOptions config = new()
            {
                AccessKey = new()
                {
                    CustomerId = CUSTOMER_ID,
                    Domain = DOMAIN,
                    ClientId = CLIENT_ID,
                },
                ServicePrincipalKey = SERVICE_PRINCIPAL_KEY,
            };
            Assert.ThrowsException<ArgumentException>(() => new ClientCredentialsClient(config));
        }

        [TestMethod]
        public void HandlerConfigurationCannotBeNull()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new ClientCredentialsClient(null));
            Assert.AreEqual(ex.Message, new ArgumentNullException("configuration").Message);
        }
    }
}
