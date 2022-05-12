using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.IdentityModel.Tokens;
using System;
using Laserfiche.Api.Client.OAuth;
using Laserfiche.Api.Client.Util;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
{
    [TestClass]
    public class ValidationTests
    {
        private const string CUSTOMER_ID = "fake.account.id";
        private const string DOMAIN = "fake.domain";
        private const string CLIENT_ID = "fake.client.id";
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
            AccessKey accessKey = new()
            {
                Domain = DOMAIN,
                ClientId = CLIENT_ID,
                Jwk = new JsonWebKey(JWK)
            };
            Assert.ThrowsException<ArgumentException>(() => accessKey.IsValid());
        }

        [TestMethod]
        public void HandlerConfigurationMissingDomain()
        {
            AccessKey accessKey = new()
            {
                CustomerId = CUSTOMER_ID,
                ClientId = CLIENT_ID,
                Jwk = new JsonWebKey(JWK)
            };
            Assert.ThrowsException<ArgumentException>(() => accessKey.IsValid());
        }

        [TestMethod]
        public void HandlerConfigurationMissingClientId()
        {
            AccessKey accessKey = new()
            {
                Domain = DOMAIN,
                CustomerId = CUSTOMER_ID,
                Jwk = new JsonWebKey(JWK)
            };
            Assert.ThrowsException<ArgumentException>(() => accessKey.IsValid());
        }
    }
}
