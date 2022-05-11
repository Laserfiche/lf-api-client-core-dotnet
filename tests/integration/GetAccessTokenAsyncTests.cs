using Laserfiche.Oauth.Token.Client;
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
            TokenApiClient client = new(AccessKey.Domain);

            // Get tokens for that application
            var response = await client.GetAccessTokenAsync(ServicePrincipalKey, AccessKey);
            Assert.IsNotNull(response);

            var tokenResponse = response.Result;

            Assert.IsNotNull(tokenResponse.Access_token);
            Assert.IsNotNull(tokenResponse.Expires_in);
            Assert.IsNotNull(tokenResponse.Token_type);
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_WrongDomain()
        {
            // Initialize an instance of the client
            TokenApiClient client = new("some.random.string");

            // Expect failed attempt to get access token since the domain is wrong
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await client.GetAccessTokenAsync(ServicePrincipalKey, AccessKey));
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            TokenApiClient client = new(AccessKey.Domain);

            // Expect the retrieval of access token to fail due to incorrect service principal key
            await Assert.ThrowsExceptionAsync<Exception>(async () => await client.GetAccessTokenAsync("a wrong service principal key", AccessKey));
        }
    }
}
