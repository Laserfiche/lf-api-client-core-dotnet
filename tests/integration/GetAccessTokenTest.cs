using Laserfiche.Oauth.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Laserfiche.OAuth.Client.ClientCredentials.IntegrationTest
{
    [TestClass]
    public class GetAccessTokenTest : BaseTest
    {
        [TestMethod]
        public async Task GetAccessToken()
        {
            // Initialize an instance of the handler
            ClientCredentialsHandler handler = new ClientCredentialsHandler(Configuration);

            // Get access token for that application
            var accessToken = await handler.GetAccessToken();
            Assert.IsNotNull(accessToken);
        }

        [TestMethod]
        public async Task GetAccessAsync_WrongBaseAddress()
        {
            // Initialize an instance of the handler
            ClientCredentialsHandler handler = new ClientCredentialsHandler(Configuration);
            handler.Configuration.BaseAddress = "https://some.random.string"; // Wrong base address.

            // Expect failed attempt to get access token since the base address is wrong
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await handler.GetAccessToken());
        }

        [TestMethod]
        public async Task GetAccessAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            ClientCredentialsHandler handler = new ClientCredentialsHandler(Configuration);
            handler.Configuration.ServicePrincipalKey = "a wrong service principal key";

            // Expect the retrieval of access token to fail due to incorrect service principal key
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await handler.GetAccessToken());
        }
    }
}
