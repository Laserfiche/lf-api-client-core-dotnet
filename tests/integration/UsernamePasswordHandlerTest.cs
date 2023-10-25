// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.HttpHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.IntegrationTest
{
    [TestClass]
    [TestCategory("APIServer")]
    public class UsernamePasswordHandlerTest : BaseTest
    {
        private IHttpRequestHandler _httpRequestHandler;

        [TestMethod]
        public async Task BeforeSendAsync_NewToken_Success()
        {
            // Arrange
            _httpRequestHandler = new UsernamePasswordHandler(RepoId, Username, Password, BaseUrl);
            using var request = new HttpRequestMessage();

            // Act
            var result = await _httpRequestHandler.BeforeSendAsync(request, default).ConfigureAwait(false);

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
            _httpRequestHandler = new UsernamePasswordHandler(RepoId, Username, Password, BaseUrl);
            using var request1 = new HttpRequestMessage();
            using var request2 = new HttpRequestMessage();

            // Act
            var result1 = await _httpRequestHandler.BeforeSendAsync(request1, default).ConfigureAwait(false);
            var result2 = await _httpRequestHandler.BeforeSendAsync(request2, default).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result2);
            Assert.IsTrue(string.IsNullOrEmpty(result2?.RegionalDomain));
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.AreEqual(request1.Headers.Authorization.Parameter, request2.Headers.Authorization.Parameter);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidCredentials), DynamicDataSourceType.Method)]
        public async Task BeforeSendAsync_FailedAuthentication_ThrowsException(string repoId, string username, string password, HttpStatusCode status)
        {
            //Arrange
            _httpRequestHandler = new UsernamePasswordHandler(repoId, username, password, BaseUrl);
            using var request = new HttpRequestMessage();

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(() => _httpRequestHandler.BeforeSendAsync(request, new CancellationToken())).ConfigureAwait(false);
            Assert.AreEqual((int)status, ex.ProblemDetails.Status);
            Assert.AreEqual(ex.ProblemDetails.Status, ex.StatusCode);
            Assert.IsNotNull(ex.ProblemDetails.Type);
            Assert.IsNotNull(ex.ProblemDetails.Title);
            Assert.AreEqual(ex.ProblemDetails.Title, ex.Message);
            Assert.IsNotNull(ex.ProblemDetails.OperationId);
        }

        private static IEnumerable<object[]> GetInvalidCredentials()
        {
            BaseTest baseTest = new();

            yield return new object[]
            {
                baseTest.RepoId, "fake123", baseTest.Password, HttpStatusCode.Unauthorized
            };
            yield return new object[]
            {
                baseTest.RepoId, baseTest.Username, "fake123", HttpStatusCode.Unauthorized
            };
            yield return new object[]
            {
                "fake123", baseTest.Username, baseTest.Password, HttpStatusCode.NotFound
            };
        }

        [TestMethod]
        public async Task AfterSendAsync_TokenRemovedWhenUnauthorized()
        {
            // Arrange
            _httpRequestHandler = new UsernamePasswordHandler(RepoId, Username, Password, BaseUrl);
            using var request1 = new HttpRequestMessage();
            var result1 = await _httpRequestHandler.BeforeSendAsync(request1, default).ConfigureAwait(false);

            // Act
            var retry = await _httpRequestHandler.AfterSendAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized), default).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(retry);
            using var request2 = new HttpRequestMessage();
            var result2 = await _httpRequestHandler.BeforeSendAsync(request2, default).ConfigureAwait(false);
            Assert.IsNotNull(result2);
            Assert.IsTrue(string.IsNullOrEmpty(result2?.RegionalDomain));
            Assert.AreEqual("Bearer", request2.Headers.Authorization.Scheme);
            Assert.IsNotNull(request2.Headers.Authorization.Parameter);
            Assert.AreNotEqual(request1.Headers.Authorization.Parameter, request2.Headers.Authorization.Parameter);
        }
    }
}
