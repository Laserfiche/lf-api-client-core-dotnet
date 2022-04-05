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
            ClientCredentialsHandler handler = new ClientCredentialsHandler(Configuration);

            // Get tokens for that application
            (string accessToken, string refreshToken) = await handler.RefreshAccessTokenAsync("");
            Assert.IsNotNull(accessToken);
            Assert.AreEqual(string.Empty, refreshToken);
        }

        [TestMethod]
        public async Task RefreshAccessTokenAsync_WrongDomain()
        {
            // Initialize an instance of the handler
            ClientCredentialsHandler handler = new ClientCredentialsHandler(Configuration);
            handler.Configuration.Domain = "some.random.string";

            // Expect failed attempt to refresh access token since the domain is wrong
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await handler.RefreshAccessTokenAsync(""));
        }

        [TestMethod]
        public async Task RefreshAccessTokenAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            ClientCredentialsHandler handler = new ClientCredentialsHandler(Configuration);
            handler.Configuration.ServicePrincipalKey = "a wrong service principal key";

            // Expect the refresh access token to fail due to incorrect service principal key
            await Assert.ThrowsExceptionAsync<Exception>(async () => await handler.RefreshAccessTokenAsync(""));
        }
    }
}
