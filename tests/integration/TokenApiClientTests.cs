using Laserfiche.Api.Client.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.IntegrationTest
{
    [TestClass]
    public class TokenApiClientTests : BaseTest
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
            Assert.IsNull(tokenResponse.Refresh_token);
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_WrongDomain()
        {
            // Initialize an instance of the client
            TokenApiClient client = new("some.random.string");

            // Expect failed attempt to get access token since the domain is wrong
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await client.GetAccessTokenAsync(ServicePrincipalKey, AccessKey));
        }

        [TestMethod]
        public async Task GetAccessTokenAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            TokenApiClient client = new(AccessKey.Domain);

            // Expect the retrieval of access token to fail due to incorrect service principal key
            await Assert.ThrowsExceptionAsync<ApiException<ProblemDetails>>(async () => await client.GetAccessTokenAsync("a wrong service principal key", AccessKey));
        }
    }
}
