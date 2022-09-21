using Microsoft.VisualStudio.TestTools.UnitTesting;
using Laserfiche.Api.Client.HttpHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using Laserfiche.Api.Client.SelfHosted;
using System.Net;

namespace Laserfiche.Api.Client.IntegrationTest
{
    [TestClass]
    [TestCategory("SelfHosted")]
    public class SelfHostedUsernamePasswordHandlerTest : BaseTest
    {
        private IHttpRequestHandler _httpRequestHandler;
        private List<string> _accessTokensToCleanUp;

        [TestInitialize]
        public void Initalize()
        {
            _accessTokensToCleanUp = new();
        }

        [TestCleanup]
        public async Task SessionCleanUp()
        {
            foreach (var accessToken in _accessTokensToCleanUp)
            {
                await LogOut(accessToken);
            }
        }

        private async Task LogOut(string accessToken)
        {
            using HttpClient client = new();
            client.BaseAddress = new Uri(BaseUrl);
            string uri = $"v1/Repositories/{RepoId}/AccessTokens/Invalidate";
            HttpRequestMessage request = new(HttpMethod.Post, uri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            await client.SendAsync(request);
        }

        [TestMethod]
        public async Task BeforeSendAsync_NewToken_Success()
        {
            // Arrange
            _httpRequestHandler = new SelfHostedUsernamePasswordHandler(Username, Password, GrantType, RepoId, BaseUrl);
            using var request = new HttpRequestMessage();

            // Act
            var result = await _httpRequestHandler.BeforeSendAsync(request, default);
            _accessTokensToCleanUp.Add(request.Headers.Authorization.Parameter);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result?.RegionalDomain));
            Assert.AreEqual("Bearer", request.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrEmpty(request.Headers.Authorization.Parameter));
        }

        [TestMethod]
        public async Task BeforeSendAsync_ExistingToken_Success()
        {
            // Arrange
            _httpRequestHandler = new SelfHostedUsernamePasswordHandler(Username, Password, GrantType, RepoId, BaseUrl);
            using var request1 = new HttpRequestMessage();
            using var request2 = new HttpRequestMessage();

            // Act
            var result1 = await _httpRequestHandler.BeforeSendAsync(request1, default);
            _accessTokensToCleanUp.Add(request1.Headers.Authorization.Parameter);

            var result2 = await _httpRequestHandler.BeforeSendAsync(request2, default);
            _accessTokensToCleanUp.Add(request2.Headers.Authorization.Parameter);

            // Assert
            Assert.IsNotNull(result2);
            Assert.IsTrue(string.IsNullOrEmpty(result2?.RegionalDomain));
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.AreEqual(request1.Headers.Authorization.Parameter, request2.Headers.Authorization.Parameter);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidCredentials), DynamicDataSourceType.Method)]
        public async Task BeforeSendAsync_FailedAuthentication_ThrowsException(string username, string password, string grantType, string repoId, HttpStatusCode status)
        {
            //Arrange
            _httpRequestHandler = new SelfHostedUsernamePasswordHandler(username, password, grantType, repoId, BaseUrl);
            using var request = new HttpRequestMessage();

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<ApiException<ProblemDetails>>(() => _httpRequestHandler.BeforeSendAsync(request, new CancellationToken()));
            Assert.AreEqual((int)status, ex.Result.Status);
            Assert.IsNotNull(ex.Result.Type);
            Assert.IsNotNull(ex.Result.Title);
        }

        private static IEnumerable<object[]> GetInvalidCredentials()
        {
            BaseTest baseTest = new();

            yield return new object[]
            {
                "fake123", baseTest.Password, baseTest.GrantType, baseTest.RepoId, HttpStatusCode.Unauthorized
            };
            yield return new object[]
            {
                baseTest.Username, "fake123", baseTest.GrantType, baseTest.RepoId, HttpStatusCode.Unauthorized
            };
            yield return new object[]
            {
                baseTest.Username, baseTest.Password, "fake123", baseTest.RepoId, HttpStatusCode.BadRequest
            };
            yield return new object[]
            {
                baseTest.Username, baseTest.Password, baseTest.GrantType, "fake123", HttpStatusCode.NotFound
            };
        }

        [TestMethod]
        public async Task AfterSendAsync_TokenRemovedWhenUnauthorized()
        {
            // Arrange
            _httpRequestHandler = new SelfHostedUsernamePasswordHandler(Username, Password, GrantType, RepoId, BaseUrl);
            using var request1 = new HttpRequestMessage();
            var result1 = await _httpRequestHandler.BeforeSendAsync(request1, default);
            _accessTokensToCleanUp.Add(request1.Headers.Authorization.Parameter);

            // Act
            var retry = await _httpRequestHandler.AfterSendAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized), default);

            // Assert
            Assert.IsTrue(retry);
            using var request2 = new HttpRequestMessage();
            var result2 = await _httpRequestHandler.BeforeSendAsync(request2, default);
            _accessTokensToCleanUp.Add(request2.Headers.Authorization.Parameter);
            Assert.IsNotNull(result2);
            Assert.IsTrue(string.IsNullOrEmpty(result2?.RegionalDomain));
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.IsNotNull(request2.Headers.Authorization.Parameter);
            Assert.AreNotEqual(request1.Headers.Authorization.Parameter, request2.Headers.Authorization.Parameter);
        }
    }
}
