using Laserfiche.Api.Client.HttpHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client.Lfds;
using System.Net;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class LfdsUsernamePasswordHandlerTest
    {
        private LfdsUsernamePasswordHandler _handler;
        private readonly string _username = "admin";
        private readonly string _password = "a";
        private readonly string _baseUri = "http://localhost:11211";
        private readonly string _repoId = "release104";
        private readonly string _organization = "ROOT";
        private readonly HttpRequestMessage _request = new HttpRequestMessage();

        [TestMethod]
        public async Task BeforeSendAsync_NewToken_Success()
        {
            // Arrange
            string accessToken = "access_token";
            Mock<IAccessTokensApiClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new SessionKeyInfo
            {
                AuthToken = accessToken
            }));
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri, tokenClientMock.Object);
            
            // Act
            var result = await _handler.BeforeSendAsync(_request, new CancellationToken());

            // Assert
            tokenClientMock.Verify(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual($"Bearer {accessToken}", _request.Headers.Authorization.ToString());
            Assert.IsNotNull(result);
            Assert.IsNull(result.RegionalDomain);
        }

        [TestMethod]
        public async Task BeforeSendAsync_ExistingToken_Success()
        {
            //Arrange
            string accessToken = "access_token";
            Mock<IAccessTokensApiClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new SessionKeyInfo
            {
                AuthToken = accessToken
            }));
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri, tokenClientMock.Object);

            // Act
            var result = await _handler.BeforeSendAsync(_request, new CancellationToken());
            result = await _handler.BeforeSendAsync(_request, new CancellationToken());

            // Assert
            tokenClientMock.Verify(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual($"Bearer {accessToken}", _request.Headers.Authorization.ToString());
            Assert.IsNotNull(result);
            Assert.IsNull(result.RegionalDomain);
        }

        [TestMethod]
        public async Task BeforeSendAsync_FailedAuthentication_ThrowsException()
        {
            //Arrange
            string message = "Access token is invalid or expired.";
            Mock<IAccessTokensApiClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new ApiException(message, 401, null, null, null));
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri, tokenClientMock.Object);

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(()=>_handler.BeforeSendAsync(_request, new CancellationToken()));
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.StatusCode);
            Assert.AreEqual($"{message}\n\nStatus: 401\nResponse: \n(null)", ex.Message);
        }

        [TestMethod]
        public async Task AfterSendAsync_ResponseOK_ReturnsFalse()
        {
            // Arrange
            HttpResponseMessage responseOk = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
            };
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri);

            // Act
            var result = await _handler.AfterSendAsync(responseOk, new CancellationToken());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AfterSendAsync_ResponseUnauthorized_ReturnsTrue()
        {
            // Arrange
            HttpResponseMessage responseOk = new()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
            };
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri);

            // Act
            var result = await _handler.AfterSendAsync(responseOk, new CancellationToken());

            // Assert
            Assert.IsTrue(result);
        }
    }
}
