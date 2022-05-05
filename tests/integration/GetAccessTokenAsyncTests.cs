using Laserfiche.Oauth.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laserfiche.OAuth.Client.ClientCredentials.IntegrationTest
{
    [TestClass]
    public class GetAccessTokenAsyncTests : BaseTest
    {
        [TestMethod]
        public async Task GetAccessTokenAsync()
        {
            // Initialize an instance of the handler
            TokenApiClient client = new(Configuration);

            // Get tokens for that application
            var tokenResponse = await client.GetAccessTokenAsync();
            Assert.IsNotNull(tokenResponse);
            Assert.IsNotNull(tokenResponse.AccessToken);
            Assert.IsNull(tokenResponse.RefreshToken);
            Assert.IsNotNull(tokenResponse.ExpiresIn);
            Assert.IsNotNull(tokenResponse.TokenType);
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_WrongDomain()
        {
            // Initialize an instance of the handler
            TokenApiClient client = new(Configuration);
            client.Configuration.Domain = "some.random.string";

            // Expect failed attempt to get access token since the domain is wrong
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await client.GetAccessTokenAsync());
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            TokenApiClient client = new(Configuration);
            client.Configuration.ServicePrincipalKey = "a wrong service principal key";

            // Expect the retrieval of access token to fail due to incorrect service principal key
            await Assert.ThrowsExceptionAsync<Exception>(async () => await client.GetAccessTokenAsync());
        }
    }
}
