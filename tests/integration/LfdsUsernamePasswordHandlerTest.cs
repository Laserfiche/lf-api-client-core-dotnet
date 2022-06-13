using Microsoft.VisualStudio.TestTools.UnitTesting;
using Laserfiche.Api.Client.HttpHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace Laserfiche.Api.Client.IntegrationTest
{
    [TestClass]
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

        //[DataTestMethod]
        //[DataRow("invalid_name", , Organization, RepoId, BaseUrl)]
        //public async Task BeforeSendAsync_FailedAuthentication_ThrowsException(string username, string password, string organization, string repoId, string baseUrl)
        //{
        //    //Arrange
        //    _httpRequestHandler = new LfdsUsernamePasswordHandler(username, password, organization, repoId, baseUrl);

        //    // Assert
        //    var ex = await Assert.ThrowsExceptionAsync<ApiException>(() => _handler.BeforeSendAsync(_request, new CancellationToken()));
        //    Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.StatusCode);
        //    Assert.IsNotNull(ex.Message);
        //}


    }
}
