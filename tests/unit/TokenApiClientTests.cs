using Laserfiche.Oauth.Api.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
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

        ITokenApiClient client;
        Mock<IHttpClientFactory> mockHttpClientFactory;
        Mock<HttpMessageHandler> mockHttpMessageHandler;
        ClientCredentialsOptions options;
        HttpClient httpClient;
        JsonWebKey accessKey;

        [TestInitialize]
        public void Setup()
        {
            mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            httpClient = new HttpClient(mockHttpMessageHandler.Object);
            options = new ClientCredentialsOptions();
            accessKey = new JsonWebKey(ACCESS_KEY);

            // Populate the handler configuration with fake information
            options.CustomerId = CUSTOMER_ID;
            options.Domain = DOMAIN;
            options.ClientId = CLIENT_ID;
            options.ServicePrincipalKey = SERVICE_PRINCIPAL_KEY;
            options.Jwk = accessKey;


            // When called, it gives a stubbed HttpClient
            mockHttpClientFactory.Setup(mock => mock.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        [TestMethod]
        public void GetAccessTokenAsync_Success()
        {
            var accessTokenResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"fake.access.token\",\"expires_in\":1001,\"token_type\":\"bearer\"}")
            };

            // Accommodate the request to get access token:
            // We expect the path is for token request.
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accessTokenResponse);

            client = new TokenApiClient(options, mockHttpClientFactory.Object);
            Assert.IsNotNull(client.GetAccessTokenAsync());
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_ExceptionResponse()
        {
            var statusCode = System.Net.HttpStatusCode.BadRequest;
            var responseContent = new OAuthProblemDetails()
            {
                Type = "invalid_client",
                Title = "The client_id is invalid or authentication failed.",
                Status = (int)statusCode,
                Instance = "/Token",
                OperationId = "fake.operation.id",
            };
            var accessTokenResponse = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseContent))
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accessTokenResponse);

            client = new TokenApiClient(options, mockHttpClientFactory.Object);
            var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await client.GetAccessTokenAsync());
            Assert.AreEqual(exception.Data["Type"], responseContent.Type);
            Assert.AreEqual(exception.Data["Title"], responseContent.Title);
            Assert.AreEqual(exception.Data["Status"], ((int)statusCode).ToString());
            Assert.AreEqual(exception.Data["Instance"], responseContent.Instance);
            Assert.AreEqual(exception.Data["OperationId"], responseContent.OperationId);
        }

        [TestMethod]
        public void RefreshAccessTokenAsync_Success()
        {
            var accessTokenResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"fake.access.token\",\"expires_in\":1001,\"token_type\":\"bearer\"}")
            };

            // Accommodate the request to get access token:
            // We expect the path is for token request.
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accessTokenResponse);

            client = new TokenApiClient(options, mockHttpClientFactory.Object);
            Assert.IsNotNull(client.RefreshAccessTokenAsync(""));
        }

        [TestMethod]
        public async Task RefreshAccessTokenAsync_ExceptionResponse()
        {
            var statusCode = System.Net.HttpStatusCode.BadRequest;
            var responseContent = new OAuthProblemDetails()
            {
                Type = "invalid_client",
                Title = "The client_id is invalid or authentication failed.",
                Status = (int)statusCode,
                Instance = "/Token",
                OperationId = "fake.operation.id",
            };
            var accessTokenResponse = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseContent))
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accessTokenResponse);

            client = new TokenApiClient(options, mockHttpClientFactory.Object);
            var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await client.RefreshAccessTokenAsync(""));
            Assert.AreEqual(exception.Data["Type"], responseContent.Type);
            Assert.AreEqual(exception.Data["Title"], responseContent.Title);
            Assert.AreEqual(exception.Data["Status"], ((int)statusCode).ToString());
            Assert.AreEqual(exception.Data["Instance"], responseContent.Instance);
            Assert.AreEqual(exception.Data["OperationId"], responseContent.OperationId);
        }
    }
}
