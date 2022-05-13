using Laserfiche.Api.Client.OAuth;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class TokenApiClientTest
    {
        private const string CUSTOMER_ID = "fake.customer.id";
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

        Mock<TokenApiClient> mockTokenApiClient;
        string servicePrincipalKey;
        AccessKey accessKey;

        [TestInitialize]
        public void Setup()
        {
            mockTokenApiClient = new Mock<TokenApiClient>(DOMAIN);
            mockTokenApiClient.Setup(tokenApiClient => tokenApiClient.TokenAsync(It.IsAny<GetAccessTokenRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new SwaggerResponse<GetAccessTokenResponse>(200, null, null)));

            servicePrincipalKey = SERVICE_PRINCIPAL_KEY;
            accessKey = new AccessKey()
            {
                CustomerId = CUSTOMER_ID,
                Domain = DOMAIN,
                ClientId = CLIENT_ID,
                Jwk = new JsonWebKey(ACCESS_KEY)
            };
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_Success()
        {
            TokenApiClient client = mockTokenApiClient.Object;
            await client.GetAccessTokenAsync(servicePrincipalKey, accessKey);
            mockTokenApiClient.Verify(tokenApiClient => tokenApiClient.TokenAsync(It.Is<GetAccessTokenRequest>(request => request.Grant_type == "client_credentials"), It.IsNotNull<string>(), It.IsAny<CancellationToken>()));
        }
    }
}
