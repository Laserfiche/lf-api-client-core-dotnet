using Microsoft.VisualStudio.TestTools.UnitTesting;
using Laserfiche.Api.Client.HttpHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using Laserfiche.Api.Client.Lfds;
using System.Net;

namespace Laserfiche.Api.Client.IntegrationTest
{
    [TestClass]
    [TestCategory("LFDS")]
    public class LfdsUsernamePasswordHandlerTest : BaseTest
    {
        private IHttpRequestHandler _httpRequestHandler;

        [TestMethod]
        public async Task BeforeSendAsync_NewToken_Success()
        {
            // Arrange
            _httpRequestHandler = new LfdsUsernamePasswordHandler(Username, Password, Organization, RepoId, BaseUrl);
            using var request = new HttpRequestMessage();

            // Act
            var result = await _httpRequestHandler.BeforeSendAsync(request, default);

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
            _httpRequestHandler = new LfdsUsernamePasswordHandler(Username, Password, Organization, RepoId, BaseUrl);
            using var request1 = new HttpRequestMessage();
            using var request2 = new HttpRequestMessage();

            // Act
            var result1 = await _httpRequestHandler.BeforeSendAsync(request1, default);
            var result2 = await _httpRequestHandler.BeforeSendAsync(request2, default);

            // Assert
            Assert.IsNotNull(result2);
            Assert.IsTrue(string.IsNullOrEmpty(result2?.RegionalDomain));
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.AreEqual(request1.Headers.Authorization.Parameter, request2.Headers.Authorization.Parameter);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidCredentials), DynamicDataSourceType.Method)]
        public async Task BeforeSendAsync_FailedAuthentication_ThrowsException(string username, string password, string organization)
        {
            //Arrange
            _httpRequestHandler = new LfdsUsernamePasswordHandler(username, password, RepoId, organization, BaseUrl);
            using var request = new HttpRequestMessage();

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<ApiException<APIServerException>>(() => _httpRequestHandler.BeforeSendAsync(request, new CancellationToken()));
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.StatusCode);
            Assert.IsNotNull(ex.Message);
        }

        private static IEnumerable<object[]> GetInvalidCredentials()
        {
            BaseTest baseTest = new();

            yield return new object[]
            {
                "invalid name", baseTest.Password, baseTest.Organization
            };
            yield return new object[]
            {
                baseTest.Username, "invalid password", baseTest.Organization
            };
            yield return new object[]
            {
                baseTest.Username, baseTest.Password, "invalid organization"
            };
        }

        [TestMethod]
        public async Task AfterSendAsync_TokenRemovedWhenUnauthorized()
        {
            // Arrange
            _httpRequestHandler = new LfdsUsernamePasswordHandler(Username, Password, Organization, RepoId, BaseUrl);
            using var request1 = new HttpRequestMessage();
            var result1 = await _httpRequestHandler.BeforeSendAsync(request1, default);

            // Act
            var retry = await _httpRequestHandler.AfterSendAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized), default);

            // Assert
            Assert.IsTrue(retry);
            using var request2 = new HttpRequestMessage();
            var result2 = await _httpRequestHandler.BeforeSendAsync(request2, default);
            Assert.IsNotNull(result2);
            Assert.IsTrue(string.IsNullOrEmpty(result2?.RegionalDomain));
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.IsNotNull(request2.Headers.Authorization.Parameter);
            Assert.AreNotEqual(request1.Headers.Authorization.Parameter, request2.Headers.Authorization.Parameter);
        }
    }
}
