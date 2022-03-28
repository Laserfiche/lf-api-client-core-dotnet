using Laserfiche.Oauth.Api.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
{
    [TestClass]
    public class GetAccessTokenTest
    {
        // Fake values
        readonly string fakeClientId = "fake.client.id";
        readonly string fakeBaseAddress = "https://fake.base.address";
        readonly string fakeSpKey = "fake.sp.key";

        IClientCredentialsHandler handler;
        Mock<IHttpClientFactory> mockHttpClientFactory;
        Mock<HttpMessageHandler> mockHttpMessageHandler;
        Mock<IClientCredentialsOptions> mockConfig;
        HttpClient httpClient;
        JsonWebKey signingKey;

        [TestInitialize]
        public void Setup()
        {
            mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockConfig = new Mock<IClientCredentialsOptions>();
            httpClient = new HttpClient(mockHttpMessageHandler.Object);
            signingKey = new JsonWebKey("{\"kty\":\"EC\",\"crv\":\"P-256\",\"use\":\"sig\",\"kid\":\"thl3d8mQVz62_cHC8P94QVJXQBmqrupX0gXCEtH4ANE\",\"X\":\"hswPzqiRo1Vsd8SxBCLIXFSqZUpXPf3xcsnV-fGycM0\",\"Y\":\"_WPNY_6EMxy6zR8DSk6bag_HQEk2W6OTJzF_LjEMMl8\",\"D\":\"Mc0vz1XdhrwvnEGVVYHU9NseusqOjVT51GGBZ_HV5Zo\",\"createdTime\":\"2021-11-03T13:48:26.5810162Z\"}");

            // So the hanlder configuration passes validation
            mockConfig.Setup(mock => mock.IsValid()).Returns((true, new List<string>()));

            // Populate the handler configuration with fake information
            mockConfig.Setup(mock => mock.ClientId).Returns(fakeClientId);
            mockConfig.Setup(mock => mock.BaseAddress).Returns(fakeBaseAddress);
            mockConfig.Setup(mock => mock.ServicePrincipalKey).Returns(fakeSpKey);
            mockConfig.Setup(mock => mock.SigningKey).Returns(signingKey);
            
            // When called, it gives a stubbed HttpClient
            mockHttpClientFactory.Setup(mock => mock.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        [TestMethod]
        public void GetAccessToken_Success()
        {
            var accessTokenResponse = new HttpResponseMessage()
            {
                Content = new StringContent("{\"access_token\":\"eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJUZXN0T0F1dGhTZXJ2aWNlUHJpbmNpcGFsIiwiY2xpZW50X2lkIjoiM3ZNdko0Ym9KNG5jUzZxNWVKcU9pTk5FIiwiY3NpZCI6IjEyMzQ1Njc4OSIsInRyaWQiOiIxMDI2IiwibmFtZSI6IlRlc3RPQXV0aFNlcnZpY2VQcmluY2lwYWwiLCJ1dHlwIjoiU2VydmljZVByaW5jaXBhbCIsImd0aWQiOiIxMDc1MzIiLCJhdWQiOiJ1cy5sYXNlcmZpY2hlLmNvbSIsImV4cCI6MTYzNTk1MjE0NSwiaXNzIjoic2lnbmluLmxhc2VyZmljaGUuY29tIiwibmJmIjoxNjM1OTUwODQ0LCJpYXQiOjE2MzU5NTExNDR9.wcOvtvySPJVCgqmugYtUVhMqBfW8J2Mjc0g14t3HhMjbVgaIx0f7GLlnEr0gPC5yBTkr2520mJQsoS0ezNX4WQ\",\"expires_in\":1001,\"token_type\":\"bearer\"}")
            };

            // Accommodate the request to get access token:
            // We expect the path is for token request.
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accessTokenResponse);

            handler = new ClientCredentialsHandler(mockConfig.Object, mockHttpClientFactory.Object);
            Assert.IsNotNull(handler.GetAccessToken());
        }

        [TestMethod]
        public async Task GetAccessToken_WrongClientId()
        {
            var accessTokenResponse = new HttpResponseMessage()
            {
                Content = new StringContent("{\"type\":\"invalid_client\",\"title\":\"The client_id is invalid or authentication failed.\",\"status\":401,\"instance\":\"/Token\",\"operationId\":\"d9340a3dc035482cad2071418d0aeb80\",\"traceId\":\"00-16b888bc771a174f9f2cfc1dc4b8287e-47b42f0404d06d4a-00\"}")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accessTokenResponse);

            handler = new ClientCredentialsHandler(mockConfig.Object, mockHttpClientFactory.Object);
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await handler.GetAccessToken());
        }

        [TestMethod]
        public async Task GetAccessToken_WrongAccountId()
        {
            var accessTokenResponse = new HttpResponseMessage()
            {
                Content = new StringContent("{\"type\":\"resource_not_found\",\"title\":\"Application registration with the provided client id does not exist.\",\"status\":404,\"instance\":\"/api/v1/ApplicationRegistration/OSZofXg58FP86SwXRZJR32iA/Key/GenerateKey\",\"operationId\":\"cd64169fda2d45b48139b69207efa60a\",\"traceId\":\"00-c905fe2d5cba9c4195b41ee58efbecbd-4d32d95574da6a47-00\"}")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accessTokenResponse);

            handler = new ClientCredentialsHandler(mockConfig.Object, mockHttpClientFactory.Object);
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await handler.GetAccessToken());
        }
    }
}
