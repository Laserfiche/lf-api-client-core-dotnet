using Laserfiche.Api.Client.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.IntegrationTest
{
    [TestClass]
    [TestCategory("Cloud")]
    public class TokenClientTests : BaseTest
    {
        [TestMethod]
        public async Task GetAccessTokenFromServicePrincipalAsync()
        {
            // Initialize an instance of the handler
            TokenClient client = new(AccessKey.Domain);

            // Get tokens for that application
            var response = await client.GetAccessTokenFromServicePrincipalAsync(ServicePrincipalKey, AccessKey);
            Assert.IsNotNull(response);

            var tokenResponse = response;

            Assert.IsNotNull(tokenResponse.Access_token);
            Assert.IsNotNull(tokenResponse.Expires_in);
            Assert.IsNotNull(tokenResponse.Token_type);
            Assert.IsNull(tokenResponse.Refresh_token);
        }

        [TestMethod]
        public async Task GetAccessTokenFromServicePrincipalAsync_WithValidScope()
        {
            // Initialize an instance of the handler
            TokenClient client = new(AccessKey.Domain);
            string scope = "repository.Read";

            // Get tokens for that application
            var response = await client.GetAccessTokenFromServicePrincipalAsync(ServicePrincipalKey, AccessKey, scope);
            Assert.IsNotNull(response);

            var tokenResponse = response;

            Assert.IsNotNull(tokenResponse.Access_token);
            Assert.IsNotNull(tokenResponse.Expires_in);
            Assert.IsNotNull(tokenResponse.Token_type);
            Assert.IsNull(tokenResponse.Refresh_token);
            Assert.IsNotNull(tokenResponse.Scope);
            Assert.AreEqual(scope, tokenResponse.Scope);
        }

        [TestMethod]
        public async Task GetAccessTokenFromServicePrincipalAsync_WithInvalidScope()
        {
            // Initialize an instance of the handler
            TokenClient client = new(AccessKey.Domain);
            string scope = "repository.read";

            // Get tokens for that application
            var response = await client.GetAccessTokenFromServicePrincipalAsync(ServicePrincipalKey, AccessKey, scope);
            Assert.IsNotNull(response);

            var tokenResponse = response;

            Assert.IsNotNull(tokenResponse.Access_token);
            Assert.IsNotNull(tokenResponse.Expires_in);
            Assert.IsNotNull(tokenResponse.Token_type);
            Assert.IsNull(tokenResponse.Refresh_token);
            Assert.IsNull(tokenResponse.Scope);
        }

        [TestMethod]
        public async Task GetAccessTokenFromServicePrincipalAsync_WrongDomain()
        {
            // Initialize an instance of the client
            TokenClient client = new("some.random.string");

            // Expect failed attempt to get access token since the domain is wrong
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await client.GetAccessTokenFromServicePrincipalAsync(ServicePrincipalKey, AccessKey));
        }

        [TestMethod]
        public async Task GetAccessTokenFromServicePrincipalAsync_WrongServicePrincipalKey()
        {
            // Initialize an instance of the handler
            TokenClient client = new(AccessKey.Domain);

            // Expect the retrieval of access token to fail due to incorrect service principal key
            var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await client.GetAccessTokenFromServicePrincipalAsync("a wrong service principal key", AccessKey));
            Assert.AreEqual(401, exception.ProblemDetails.Status);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.IsNotNull(exception.ProblemDetails.Title);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.IsNotNull(exception.ProblemDetails.Type);
            Assert.IsNotNull(exception.ProblemDetails.OperationId);
        }
    }
}
