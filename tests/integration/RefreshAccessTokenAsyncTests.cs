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
            ClientCredentialsClient client = new(Configuration);

            // Get tokens for that application
            var tokenResponse = await client.RefreshAccessTokenAsync("");
            Assert.IsNotNull(tokenResponse);
            Assert.IsNotNull(tokenResponse.AccessToken);

            // This one might be a bit unexpected. but the client credential flow doesn't have concept of refresh token.
            // But we want to maintain a uniformity between various OAuth strategies. So the uniformed procesure is to
            // passing in a refresh token and use it in exchange for a new access token. This value is null to reflect
            // the fact with this flow, we will never get a refresh token. The user can still pass in a refresh token
            // coming from the previous result of calling GetAccessTokenAsync since RefreshAccessTokenAsync doesn't
            // care about that refresh token string.
            Assert.IsNull(tokenResponse.RefreshToken);
            
            Assert.IsNotNull(tokenResponse.ExpiresIn);
            Assert.IsNotNull(tokenResponse.TokenType);
        }

        [TestMethod]
        public async Task RefreshAccessTokenAsync_WrongDomain()
        {
            // Initialize an instance of the handler
            ClientCredentialsClient client = new(Configuration);
            client.Configuration.Domain = "some.random.string";

            // Expect failed attempt to refresh access token since the domain is wrong
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await client.RefreshAccessTokenAsync(""));
        }

        [TestMethod]
        public async Task RefreshAccessTokenAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            ClientCredentialsClient client = new(Configuration);
            client.Configuration.ServicePrincipalKey = "a wrong service principal key";

            // Expect the refresh access token to fail due to incorrect service principal key
            await Assert.ThrowsExceptionAsync<Exception>(async () => await client.RefreshAccessTokenAsync(""));
        }
    }
}
