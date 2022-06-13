using Laserfiche.Api.Client.HttpHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
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
        private readonly string _username = "username";
        private readonly string _password = "password";
        private readonly string _baseUri = "http://localhost:11211";
        private readonly string _repoId = "repoId";
        private readonly string _organization = "organization";
        private readonly HttpRequestMessage _request = new();

        [TestMethod]
        public async Task BeforeSendAsync_NewToken_Success()
        {
            // Arrange
            string accessToken = "access_token";
            Mock<IAccessTokensClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new SessionKeyInfo
            {
                AuthToken = accessToken
            }));
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri, tokenClientMock.Object);
            
            // Act
            var result = await _handler.BeforeSendAsync(_request, new CancellationToken());

            // Assert
            tokenClientMock.Verify(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual("Bearer", _request.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrEmpty(_request.Headers.Authorization.Parameter));
            Assert.IsNotNull(result);
            Assert.IsNull(result.RegionalDomain);
        }

        [TestMethod]
        public async Task BeforeSendAsync_ExistingToken_Success()
        {
            //Arrange
            string accessToken = "access_token";
            Mock<IAccessTokensClient> tokenClientMock = new();
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
            Assert.AreEqual("Bearer", _request.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrEmpty(_request.Headers.Authorization.Parameter));
            Assert.IsNotNull(result);
            Assert.IsNull(result.RegionalDomain);
        }

        [TestMethod]
        public async Task BeforeSendAsync_FailedAuthentication_ThrowsException()
        {
            //Arrange
            string message = "Access token is invalid or expired.";
            Mock<IAccessTokensClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.CreateAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new ApiException(message, 401, null, null, null));
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri, tokenClientMock.Object);

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(()=>_handler.BeforeSendAsync(_request, new CancellationToken()));
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.StatusCode);
            Assert.IsNotNull(ex.Message);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.OK)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task AfterSendAsync_ResponseOtherThanUnauthorized_ReturnsFalse(HttpStatusCode statusCode)
        {
            // Arrange
            HttpResponseMessage response = new()
            {
                StatusCode = statusCode,
            };
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri);

            // Act
            var result = await _handler.AfterSendAsync(response, new CancellationToken());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AfterSendAsync_ResponseUnauthorized_ReturnsTrue()
        {
            // Arrange
            HttpResponseMessage response = new()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
            };
            _handler = new LfdsUsernamePasswordHandler(_username, _password, _organization, _repoId, _baseUri);

            // Act
            var result = await _handler.AfterSendAsync(response, new CancellationToken());

            // Assert
            Assert.IsTrue(result);
        }
    }
}
