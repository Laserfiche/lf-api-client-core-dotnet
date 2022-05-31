using Laserfiche.Api.Client.HttpHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.IntegrationTest
{
    [TestClass]
    public class OAuthClientCredentialsHandlerTests : BaseTest
    {
        [TestMethod]
        public async Task BeforeSendAsync_Fail()
        {
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public async Task BeforeSendAsync_Success()
        {
            var httpRequestHandler = new OAuthClientCredentialsHandler(ServicePrincipalKey, AccessKey);
            using var request = new System.Net.Http.HttpRequestMessage();

            var result = await httpRequestHandler.BeforeSendAsync(request, default);

            Assert.AreEqual("Bearer", request.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrWhiteSpace(request.Headers.Authorization.Parameter));
            Assert.AreEqual(AccessKey.Domain, result.RegionalDomain);
        }

        [TestMethod]
        public async Task BeforeSendAsync_Twice_GetAccessTokenOnce()
        {
            var httpRequestHandler = new OAuthClientCredentialsHandler(ServicePrincipalKey, AccessKey);
            using var request1 = new System.Net.Http.HttpRequestMessage();
            using var request2 = new System.Net.Http.HttpRequestMessage();

            var result1 = await httpRequestHandler.BeforeSendAsync(request1, default);
            var result2 = await httpRequestHandler.BeforeSendAsync(request2, default);

            Assert.AreEqual("Bearer", request1.Headers.Authorization.Scheme);
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrWhiteSpace(request1.Headers.Authorization.Parameter));
            Assert.AreEqual(request1.Headers.Authorization.Parameter, request2.Headers.Authorization.Parameter);
            Assert.AreEqual(AccessKey.Domain, result1.RegionalDomain);
            Assert.AreEqual(AccessKey.Domain, result2.RegionalDomain);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.OK)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task AfterSendAsync_DoNotRetry(HttpStatusCode statusCode)
        {
            var httpRequestHandler = new OAuthClientCredentialsHandler(ServicePrincipalKey, AccessKey);
            using var response = new System.Net.Http.HttpResponseMessage(statusCode);

            var retry = await httpRequestHandler.AfterSendAsync(response, default);

            Assert.IsFalse(retry);
        }

        [TestMethod]
        public async Task AfterSendAsync_DoRetry()
        {
            var httpRequestHandler = new OAuthClientCredentialsHandler(ServicePrincipalKey, AccessKey);
            using var response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);

            var retry = await httpRequestHandler.AfterSendAsync(response, default);

            Assert.IsTrue(retry);
        }

        [TestMethod]
        public async Task AfterSendAsync_DoRetry_AccessTokenRemoved()
        {
            var httpRequestHandler = new OAuthClientCredentialsHandler(ServicePrincipalKey, AccessKey);

            // Get an access token
            using var request1 = new System.Net.Http.HttpRequestMessage();
            var result1 = await httpRequestHandler.BeforeSendAsync(request1, default);
            Assert.AreEqual("Bearer", request1.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrWhiteSpace(request1.Headers.Authorization.Parameter));
            Assert.AreEqual(AccessKey.Domain, result1.RegionalDomain);
            var accessToken1 = request1.Headers.Authorization.Parameter;

            // Remove the access token
            using var response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);
            var retry = await httpRequestHandler.AfterSendAsync(response, default);
            Assert.IsTrue(retry);

            // Get a new access token
            using var request2 = new System.Net.Http.HttpRequestMessage();
            var result2 = await httpRequestHandler.BeforeSendAsync(request2, default);
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrWhiteSpace(request2.Headers.Authorization.Parameter));
            Assert.AreEqual(AccessKey.Domain, result2.RegionalDomain);
            var accessToken2 = request2.Headers.Authorization.Parameter;

            Assert.AreNotEqual(accessToken1, accessToken2);
        }
    }
}
