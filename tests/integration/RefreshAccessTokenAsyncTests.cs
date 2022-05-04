using Laserfiche.Oauth.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laserfiche.OAuth.Client.ClientCredentials.IntegrationTest
{
    [TestClass]
    public class RefreshAccessTokenAsyncTests : BaseTest
    {
        [TestMethod]
        public async Task RefreshAccessTokenAsync()
        {
            // Initialize an instance of the handler
            TokenApiClient handler = new(Configuration);

            // Get tokens for that application
            var tokenResponse = await handler.RefreshAccessTokenAsync("");
            Assert.IsNotNull(tokenResponse);
            Assert.IsNotNull(tokenResponse.RefreshToken);
            Assert.IsNotNull(tokenResponse.ExpiresIn);
            Assert.IsNotNull(tokenResponse.TokenType);
        }

        [TestMethod]
        public async Task RefreshAccessTokenAsync_WrongDomain()
        {
            // Initialize an instance of the handler
            TokenApiClient client = new(Configuration);
            client.Configuration.Domain = "some.random.string";

            // Expect failed attempt to refresh access token since the domain is wrong
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await client.RefreshAccessTokenAsync(""));
        }

        [TestMethod]
        public async Task RefreshAccessTokenAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            TokenApiClient client = new(Configuration);
            client.Configuration.ServicePrincipalKey = "a wrong service principal key";

            // Expect the refresh access token to fail due to incorrect service principal key
            await Assert.ThrowsExceptionAsync<Exception>(async () => await client.RefreshAccessTokenAsync(""));
        }
    }
}
